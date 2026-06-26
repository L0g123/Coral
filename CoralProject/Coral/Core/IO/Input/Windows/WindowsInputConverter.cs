using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.IO.Input.Windows
{
    using System.Runtime.InteropServices;

    public static class WindowsInputConverter
    {
        public static InputEvent? Convert(INPUT_RECORD record) => record.EventType switch
        {
            NativeMethods.KEY_EVENT => ConvertKey(record.Event.KeyEvent),
            NativeMethods.MOUSE_EVENT => ConvertMouse(record.Event.MouseEvent),
            NativeMethods.WINDOW_BUFFER_SIZE_EVENT => ConvertResize(record.Event.WindowBufferSizeEvent),
            _ => null,
        };

        // ── Keyboard ────────────────────────────────────────────────────────────

        private static InputEvent.Key ConvertKey(KEY_EVENT_RECORD e) =>
            new(new KeyEvent(
                Key: (ConsoleKey)e.wVirtualKeyCode,
                Character: e.uChar,
                Modifiers: ConvertKeyModifiers(e.dwControlKeyState),
                IsDown: e.bKeyDown != 0     // explicit int comparison, not bool cast
            ));

        // ── Mouse ────────────────────────────────────────────────────────────────

        private static uint _prevButtonState = 0;

        private static InputEvent? ConvertMouse(MOUSE_EVENT_RECORD e)
        {
            var modifiers = ConvertKeyModifiers(e.dwControlKeyState);
            int col = e.dwMousePosition.X;
            int row = e.dwMousePosition.Y;

            // Scroll wheel — dwButtonState high word carries the signed delta
            if (e.dwEventFlags == NativeMethods.MOUSE_WHEELED)
            {
                short delta = (short)(e.dwButtonState >> 16);
                var kind = delta > 0 ? MouseEventKind.ScrollUp : MouseEventKind.ScrollDown;
                return new InputEvent.Mouse(new MouseEvent(kind, MouseButton.None, col, row, modifiers));
            }

            if (e.dwEventFlags == NativeMethods.MOUSE_HWHEELED)
            {
                short delta = (short)(e.dwButtonState >> 16);
                var kind = delta > 0 ? MouseEventKind.ScrollRight : MouseEventKind.ScrollLeft;
                return new InputEvent.Mouse(new MouseEvent(kind, MouseButton.None, col, row, modifiers));
            }

            // Movement with no button change
            if (e.dwEventFlags == NativeMethods.MOUSE_MOVED &&
                e.dwButtonState == _prevButtonState)
            {
                return new InputEvent.Mouse(
                    new MouseEvent(MouseEventKind.Move, HeldButton(e.dwButtonState), col, row, modifiers));
            }

            // Button state changed — diff against previous snapshot
            uint pressed = e.dwButtonState & ~_prevButtonState;
            uint released = _prevButtonState & ~e.dwButtonState;
            _prevButtonState = e.dwButtonState;

            if (pressed != 0)
            {
                return new InputEvent.Mouse(
                    new MouseEvent(MouseEventKind.Press, ToButton(pressed), col, row, modifiers));
            }

            if (released != 0)
            {
                return new InputEvent.Mouse(
                    new MouseEvent(MouseEventKind.Release, ToButton(released), col, row, modifiers));
            }

            return null; // double-click, focus events, etc. — drop them
        }

        // ── Resize ───────────────────────────────────────────────────────────────

        private static InputEvent.Resize ConvertResize(WINDOW_BUFFER_SIZE_RECORD e) =>
            new(new ResizeEvent(e.dwSize.X, e.dwSize.Y));

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static MouseButton ToButton(uint state)
        {
            if ((state & NativeMethods.FROM_LEFT_1ST_BUTTON_PRESSED) != 0) return MouseButton.Left;
            if ((state & NativeMethods.RIGHTMOST_BUTTON_PRESSED) != 0) return MouseButton.Right;
            if ((state & NativeMethods.FROM_LEFT_2ND_BUTTON_PRESSED) != 0) return MouseButton.Middle;
            return MouseButton.None;
        }

        // Returns the button being held during a move event (if any)
        private static MouseButton HeldButton(uint state) => ToButton(state);

        private static KeyModifiers ConvertKeyModifiers(uint state)
        {
            var mods = KeyModifiers.None;
            // SHIFT_PRESSED
            if ((state & 0x0010) != 0) mods |= KeyModifiers.Shift;
            // LEFT_CTRL_PRESSED | RIGHT_CTRL_PRESSED
            if ((state & 0x0008) != 0 || (state & 0x0004) != 0) mods |= KeyModifiers.Ctrl;
            // LEFT_ALT_PRESSED  | RIGHT_ALT_PRESSED
            if ((state & 0x0002) != 0 || (state & 0x0001) != 0) mods |= KeyModifiers.Alt;
            return mods;
        }
    }
}
