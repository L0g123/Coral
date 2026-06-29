using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Coral.Core.IO.Input.Unix
{
    public class UnixInputReader : IDisposable, IInputReader
    {
        private readonly IntPtr _stdin = new IntPtr(0); // STDIN_FILENO = 0
        private termios _originalTermios;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _readTask;

        public event Action<InputEvent> EventReceived = _ => { };

        public UnixInputReader()
        {
            if (!IsTTY())
                throw new InvalidOperationException("Not running in a terminal!");

            // Get original terminal settings
            if (tcgetattr(_stdin, out _originalTermios) != 0)
                throw new InvalidOperationException("Failed to get terminal attributes!");

            // Set raw mode
            var raw = _originalTermios;
            // Input modes
            raw.c_iflag &= ~(BRKINT | ICRNL | INPCK | ISTRIP | IXON);
            // Output modes
            raw.c_oflag &= ~OPOST;
            // Control modes
            raw.c_cflag |= CS8;
            // Local modes
            raw.c_lflag &= ~(ECHO | ICANON | IEXTEN | ISIG);

            // Set VMIN/VTIME for blocking read of at least 1 byte
            raw.c_cc[VMIN] = 1;
            raw.c_cc[VTIME] = 0;

            if (tcsetattr(_stdin, TCSANOW, ref raw) != 0)
                throw new InvalidOperationException("Failed to set terminal to raw mode!");

            // Enable mouse tracking (SGR mode for the converter)
            try
            {
                Console.Write("\x1b[?1003h\x1b[?1006h"); // Any-event tracking + SGR
                Console.Out.Flush();
            }
            catch { /* ignore if can't write */ }

            _readTask = Task.Run(ReadLoop);
        }

        private bool IsTTY()
        {
            return isatty(_stdin) == 1;
        }

        private void ReadLoop()
        {
            var buffer = new byte[512];
            var pending = new List<byte>();

            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    int bytesRead = read(_stdin, buffer, (uint)buffer.Length);
                    if (bytesRead <= 0)
                    {
                        if (bytesRead < 0)
                        {
                            int err = Marshal.GetLastWin32Error();
                            if (err != EINTR) break;
                        }
                        continue;
                    }

                    pending.AddRange(new ReadOnlySpan<byte>(buffer, 0, bytesRead).ToArray());

                    int offset = 0;
                    while (offset < pending.Count)
                    {
                        var remaining = new ReadOnlySpan<byte>(pending.ToArray(), offset, pending.Count - offset);
                        int consumed = 0;
                        InputEvent? evt = null;

                        if (remaining.Length > 0)
                        {
                            if (remaining[0] == 0x1b)
                            {
                                // Scan for common escape sequence terminator
                                for (int i = 1; i < remaining.Length; i++)
                                {
                                    byte b = remaining[i];
                                    if (b == '~' || b == 'M' || b == 'm' ||
                                        (b >= (byte)'A' && b <= (byte)'Z') ||
                                        (b >= (byte)'a' && b <= (byte)'z'))
                                    {
                                        var seq = remaining.Slice(0, i + 1);
                                        evt = UnixInputConverter.Convert(seq);
                                        consumed = seq.Length;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // Plain byte (including Ctrl keys)
                                evt = UnixInputConverter.Convert(remaining.Slice(0, 1));
                                consumed = 1;
                            }
                        }

                        if (evt != null && consumed > 0)
                        {
                            EventReceived?.Invoke(evt);
                            offset += consumed;
                        }
                        else
                        {
                            // Incomplete escape sequence — wait for more data
                            break;
                        }
                    }

                    if (offset > 0)
                        pending.RemoveRange(0, offset);
                }
                catch
                {
                    break;
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();

            // Disable mouse
            try
            {
                Console.Write("\x1b[?1003l\x1b[?1006l");
                Console.Out.Flush();
            }
            catch { }

            // Restore original termios
            tcsetattr(_stdin, TCSANOW, ref _originalTermios);

            try
            {
                _readTask.Wait(TimeSpan.FromSeconds(1));
            }
            catch { }

            _cts.Dispose();
        }

        // P/Invoke
        [StructLayout(LayoutKind.Sequential)]
        private struct termios
        {
            public uint c_iflag;
            public uint c_oflag;
            public uint c_cflag;
            public uint c_lflag;
            public byte c_line;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] c_cc;
            public uint c_ispeed;
            public uint c_ospeed;
        }

        private const int TCSANOW = 0;
        private const uint BRKINT = 2u;
        private const uint ICRNL = 256u;
        private const uint INPCK = 16u;
        private const uint ISTRIP = 32u;
        private const uint IXON = 0x400u;
        private const uint OPOST = 1u;
        private const uint ECHO = 8u;
        private const uint ICANON = 2u;
        private const uint IEXTEN = 0x8000u;
        private const uint ISIG = 1u;
        private const uint CS8 = 0x30u;
        private const int VMIN = 6;
        private const int VTIME = 5;
        private const int EINTR = 4;

        [DllImport("libc", SetLastError = true)]
        private static extern int tcgetattr(IntPtr fd, out termios termios_p);

        [DllImport("libc", SetLastError = true)]
        private static extern int tcsetattr(IntPtr fd, int optional_actions, ref termios termios_p);

        [DllImport("libc", SetLastError = true)]
        private static extern int read(IntPtr fd, [Out] byte[] buf, uint count);

        [DllImport("libc", SetLastError = true)]
        private static extern int isatty(IntPtr fd);
    }
}
