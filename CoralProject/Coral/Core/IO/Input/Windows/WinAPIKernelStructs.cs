using System.Runtime.InteropServices;

// The top-level record — 'EventType' tells you which union field is active
[StructLayout(LayoutKind.Sequential)]
public struct INPUT_RECORD
{
    public ushort EventType;
    public INPUT_RECORD_EVENT Event;
}

// A union — all fields share the same memory, only one is valid at a time
[StructLayout(LayoutKind.Explicit)]
public struct INPUT_RECORD_EVENT
{
    [FieldOffset(0)] public KEY_EVENT_RECORD KeyEvent;
    [FieldOffset(0)] public MOUSE_EVENT_RECORD MouseEvent;
    [FieldOffset(0)] public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
    // ignore MENU_EVENT_RECORD and FOCUS_EVENT_RECORD, who cares
}

[StructLayout(LayoutKind.Sequential)]
public struct MOUSE_EVENT_RECORD
{
    public COORD dwMousePosition;
    public uint dwButtonState;   // which buttons are currently held
    public uint dwControlKeyState;
    public uint dwEventFlags;    // what kind of mouse event this is
}

[StructLayout(LayoutKind.Sequential)]
public struct KEY_EVENT_RECORD
{
    public int bKeyDown;
    public ushort wRepeatCount;
    public ushort wVirtualKeyCode;
    public ushort wVirtualScanCode;
    public char uChar;
    public uint dwControlKeyState;
}

[StructLayout(LayoutKind.Sequential)]
public struct WINDOW_BUFFER_SIZE_RECORD
{
    public COORD dwSize;
}

[StructLayout(LayoutKind.Sequential)]
public struct COORD
{
    public short X;
    public short Y;
}