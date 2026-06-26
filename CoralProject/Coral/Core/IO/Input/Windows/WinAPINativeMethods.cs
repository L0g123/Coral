using System.Runtime.InteropServices;

internal static class NativeMethods
{
    // Console mode flags
    public const uint ENABLE_MOUSE_INPUT = 0x0010;
    public const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
    public const uint ENABLE_EXTENDED_FLAGS = 0x0080; // required to disable quick-edit
    public const uint ENABLE_WINDOW_INPUT = 0x0008; // get resize events

    // EventType values in INPUT_RECORD
    public const ushort KEY_EVENT = 0x0001;
    public const ushort MOUSE_EVENT = 0x0002;
    public const ushort WINDOW_BUFFER_SIZE_EVENT = 0x0004;

    // dwEventFlags in MOUSE_EVENT_RECORD
    public const uint MOUSE_MOVED = 0x0001;
    public const uint DOUBLE_CLICK = 0x0002;
    public const uint MOUSE_WHEELED = 0x0004;
    public const uint MOUSE_HWHEELED = 0x0008;

    // dwButtonState bits
    public const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001; // left
    public const uint RIGHTMOST_BUTTON_PRESSED = 0x0002; // right
    public const uint FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004; // middle

    public static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle); // pass -10 for stdin

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadConsoleInput(
        IntPtr hConsoleInput,
        [Out] INPUT_RECORD[] lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead);
}