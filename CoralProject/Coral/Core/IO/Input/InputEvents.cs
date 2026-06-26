using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.IO.Input
{
    public enum MouseButton { None, Left, Middle, Right }

    public enum MouseEventKind
    {
        Press,
        Release,
        Move,
        ScrollUp,
        ScrollDown,
        ScrollLeft,   // MOUSE_HWHEELED / SGR horizontal
        ScrollRight,
    }

    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Shift = 1 << 0,
        Ctrl = 1 << 1,
        Alt = 1 << 2,
    }

    // --- Events ---

    public record MouseEvent(
        MouseEventKind Kind,
        MouseButton Button,
        int Col,
        int Row,
        KeyModifiers Modifiers
    );

    public record KeyEvent(
        ConsoleKey Key,
        char Character,  // '\0' if non-printable
        KeyModifiers Modifiers,
        bool IsDown
    );

    public record ResizeEvent(
        int Width,
        int Height
    );

    // Discriminated union — one of the three above
    public abstract record InputEvent
    {
        public record Mouse(MouseEvent Value) : InputEvent;
        public record Key(KeyEvent Value) : InputEvent;
        public record Resize(ResizeEvent Value) : InputEvent;
    }
}
