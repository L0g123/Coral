using Coral.Core.IO.Input.Windows;
using Coral.Core.IO.Input.Unix;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.IO.Input
{
    public interface IInputReader : IDisposable
    {
        event Action<InputEvent> EventReceived;
    }

    public static class InputReaderFactory
    {
        public static IInputReader Create() =>
            OperatingSystem.IsWindows()
                ? new WindowsInputReader()
                : new UnixInputReader();
    }
}
