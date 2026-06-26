namespace Coral.Core.IO.Input.Windows
{
    public class WindowsInputReader : IDisposable, IInputReader
    {
        private readonly IntPtr _handle;
        private readonly uint _originalMode;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _readTask;

        public event Action<InputEvent> EventReceived = _ => { };

        public WindowsInputReader()
        {
            _handle = NativeMethods.GetStdHandle(-10); // STD_INPUT_HANDLE
            if (_handle == NativeMethods.INVALID_HANDLE_VALUE)
                throw new InvalidOperationException("Failed to get stdin handle.");

            // Save original mode so we can restore it on exit
            if (!NativeMethods.GetConsoleMode(_handle, out _originalMode))
                throw new InvalidOperationException("Failed to get console mode.");

            var newMode = _originalMode;
            newMode |= NativeMethods.ENABLE_MOUSE_INPUT;
            newMode |= NativeMethods.ENABLE_EXTENDED_FLAGS; // required for the next line to work
            newMode |= NativeMethods.ENABLE_WINDOW_INPUT;
            newMode &= ~NativeMethods.ENABLE_QUICK_EDIT_MODE; // MUST disable — it blocks mouse events

            if (!NativeMethods.SetConsoleMode(_handle, newMode))
                throw new InvalidOperationException("Failed to set console mode.");

            _readTask = Task.Run(ReadLoop);
        }

        private void ReadLoop()
        {
            var buf = new INPUT_RECORD[16]; // read up to 16 events at once

            while (!_cts.IsCancellationRequested)
            {
                // ReadConsoleInput blocks until at least one event is available.
                // It's a blocking call so we can't pass a cancellation token directly —
                // Dispose() will unblock it by closing/restoring the handle.
                if (!NativeMethods.ReadConsoleInput(_handle, buf, (uint)buf.Length, out uint count))
                    break;

                for (uint i = 0; i < count; i++)
                {
                    var evt = WindowsInputConverter.Convert(buf[i]);
                    if (evt is not null)
                        EventReceived?.Invoke(evt);
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            // Restore the original console mode — this also unblocks ReadConsoleInput
            NativeMethods.SetConsoleMode(_handle, _originalMode);
            _readTask.Wait();
            _cts.Dispose();
        }
    }
}
