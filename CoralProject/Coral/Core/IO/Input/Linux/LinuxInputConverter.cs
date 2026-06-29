using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.IO.Input.Linux
{
    public static class LinuxInputConverter
    {
        // Entry point — takes a raw escape sequence or single byte, emits a unified event
        public static InputEvent? Convert(ReadOnlySpan<byte> raw)
        {
            if (raw.Length == 0) return null;

            // Plain ASCII key (not an escape sequence)
            if (raw[0] != 0x1b)
                return ConvertPlainKey(raw[0]);

            // Must be at least \x1b[
            if (raw.Length < 3 || raw[1] != (byte)'[')
                return new InputEvent.Key(new KeyEvent(ConsoleKey.Escape, '\x1b', KeyModifiers.None, true));

            // SGR mouse: \x1b[<btn;col;rowM  or  \x1b[<btn;col;rowm
            if (raw[2] == (byte)'<')
                return ConvertSgrMouse(raw);

            // Everything else is a keyboard escape sequence
            return ConvertEscapeKey(raw);
        }

        // ── Keyboard ─────────────────────────────────────────────────────────────

        private static InputEvent.Key ConvertPlainKey(byte b)
        {
            // Ctrl+letter comes in as raw control codes 0x01–0x1A
            if (b is >= 0x01 and <= 0x1A && b != 0x09 && b != 0x0A && b != 0x0D)
            {
                char letter = (char)('a' + b - 1);
                return new(new KeyEvent(
                    Key: CharToConsoleKey(letter),
                    Character: letter,
                    Modifiers: KeyModifiers.Ctrl,
                    IsDown: true));
            }

            var ch = (char)b;
            var key = ch switch
            {
                '\t' => ConsoleKey.Tab,
                '\n' or '\r' => ConsoleKey.Enter,
                '\x7f' => ConsoleKey.Backspace,
                ' ' => ConsoleKey.Spacebar,
                _ => CharToConsoleKey(ch),
            };

            return new(new KeyEvent(key, ch, KeyModifiers.None, IsDown: true));
        }

        private static InputEvent.Key ConvertEscapeKey(ReadOnlySpan<byte> seq)
        {
            // seq is \x1b[ + payload + terminator
            byte terminator = seq[^1];
            var payload = seq.Slice(2, seq.Length - 3); // between [ and terminator

            // Parse optional modifier parameter (e.g. \x1b[1;2A = Shift+Up)
            var mods = KeyModifiers.None;
            int semicolon = IndexOf(payload, (byte)';');
            if (semicolon >= 0 && int.TryParse(
                    System.Text.Encoding.ASCII.GetString(payload.Slice(semicolon + 1)),
                    out int modParam))
            {
                mods = ConvertXtermModifier(modParam);
            }

            var (key, ch) = terminator switch
            {
                (byte)'A' => (ConsoleKey.UpArrow, '\0'),
                (byte)'B' => (ConsoleKey.DownArrow, '\0'),
                (byte)'C' => (ConsoleKey.RightArrow, '\0'),
                (byte)'D' => (ConsoleKey.LeftArrow, '\0'),
                (byte)'H' => (ConsoleKey.Home, '\0'),
                (byte)'F' => (ConsoleKey.End, '\0'),
                (byte)'P' => (ConsoleKey.F1, '\0'),
                (byte)'Q' => (ConsoleKey.F2, '\0'),
                (byte)'R' => (ConsoleKey.F3, '\0'),
                (byte)'S' => (ConsoleKey.F4, '\0'),
                (byte)'~' => ParseTildeKey(payload),
                _ => (ConsoleKey.NoName, '\0'),
            };

            return new(new KeyEvent(key, ch, mods, IsDown: true));
        }

        private static (ConsoleKey, char) ParseTildeKey(ReadOnlySpan<byte> payload)
        {
            // Take digits before the optional semicolon
            int end = IndexOf(payload, (byte)';');
            var numSpan = end >= 0 ? payload.Slice(0, end) : payload;

            return int.TryParse(System.Text.Encoding.ASCII.GetString(numSpan), out int n)
                ? n switch
                {
                    2 => (ConsoleKey.Insert, '\0'),
                    3 => (ConsoleKey.Delete, '\0'),
                    5 => (ConsoleKey.PageUp, '\0'),
                    6 => (ConsoleKey.PageDown, '\0'),
                    15 => (ConsoleKey.F5, '\0'),
                    17 => (ConsoleKey.F6, '\0'),
                    18 => (ConsoleKey.F7, '\0'),
                    19 => (ConsoleKey.F8, '\0'),
                    20 => (ConsoleKey.F9, '\0'),
                    21 => (ConsoleKey.F10, '\0'),
                    23 => (ConsoleKey.F11, '\0'),
                    24 => (ConsoleKey.F12, '\0'),
                    _ => (ConsoleKey.NoName, '\0'),
                }
                : (ConsoleKey.NoName, '\0');
        }

        // ── Mouse ─────────────────────────────────────────────────────────────────

        private static InputEvent? ConvertSgrMouse(ReadOnlySpan<byte> seq)
        {
            // Format: \x1b[<btn;col;rowM  or  \x1b[<btn;col;rowm
            bool isRelease = seq[^1] == (byte)'m';
            var inner = seq.Slice(3, seq.Length - 4); // strip \x1b[< and terminator
            var str = System.Text.Encoding.ASCII.GetString(inner);
            var parts = str.Split(';');

            if (parts.Length != 3
                || !int.TryParse(parts[0], out int btn)
                || !int.TryParse(parts[1], out int col)
                || !int.TryParse(parts[2], out int row))
                return null;

            var modifiers = ConvertSgrModifiers(btn);
            col--; row--; // SGR is 1-indexed

            // Scroll events encode as button 64/65/66/67
            if ((btn & 64) != 0)
            {
                var scrollKind = (btn & 3) switch
                {
                    0 => MouseEventKind.ScrollUp,
                    1 => MouseEventKind.ScrollDown,
                    2 => MouseEventKind.ScrollLeft,
                    3 => MouseEventKind.ScrollRight,
                    _ => MouseEventKind.ScrollUp,
                };
                return new InputEvent.Mouse(new MouseEvent(scrollKind, MouseButton.None, col, row, modifiers));
            }

            var button = (btn & 3) switch
            {
                0 => MouseButton.Left,
                1 => MouseButton.Middle,
                2 => MouseButton.Right,
                3 => MouseButton.None, // button-release in legacy encoding; in SGR 'm' is used instead
                _ => MouseButton.None,
            };

            bool isMove = (btn & 32) != 0;
            var kind = isMove ? MouseEventKind.Move :
                       isRelease ? MouseEventKind.Release :
                                   MouseEventKind.Press;

            return new InputEvent.Mouse(new MouseEvent(kind, button, col, row, modifiers));
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        // xterm modifier param: the value is (modifiers - 1) as a bitmask
        // Shift=1, Alt=2, Ctrl=4  →  param 2=Shift, 3=Alt+Shift, 5=Ctrl, etc.
        private static KeyModifiers ConvertXtermModifier(int param)
        {
            int bits = param - 1;
            var mods = KeyModifiers.None;
            if ((bits & 1) != 0) mods |= KeyModifiers.Shift;
            if ((bits & 2) != 0) mods |= KeyModifiers.Alt;
            if ((bits & 4) != 0) mods |= KeyModifiers.Ctrl;
            return mods;
        }

        private static KeyModifiers ConvertSgrModifiers(int btn)
        {
            var mods = KeyModifiers.None;
            if ((btn & 4) != 0) mods |= KeyModifiers.Shift;
            if ((btn & 8) != 0) mods |= KeyModifiers.Alt;
            if ((btn & 16) != 0) mods |= KeyModifiers.Ctrl;
            return mods;
        }

        private static ConsoleKey CharToConsoleKey(char c) =>
            char.ToUpper(c) is >= 'A' and <= 'Z'
                ? ConsoleKey.A + (char.ToUpper(c) - 'A')
                : ConsoleKey.NoName;

        private static int IndexOf(ReadOnlySpan<byte> span, byte value)
        {
            for (int i = 0; i < span.Length; i++)
                if (span[i] == value) return i;
            return -1;
        }
    }
}
