using Coral.Core.IO.Input.Windows;
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
                // TODO: UnixInputReader (see InputEvent, UnixInputConverter classes)
                : throw new NotImplementedException("Missing class UnixInputReader");
    }
}
