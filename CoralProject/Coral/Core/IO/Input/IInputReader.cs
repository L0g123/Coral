using Coral.Core.IO.Input.Windows;
using Coral.Core.IO.Input.Linux;
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

        public static IInputReader Create()
        {
            if (OperatingSystem.IsWindows())
            {
                return new WindowsInputReader();
            }
            else if (OperatingSystem.IsLinux())
            {
                return new LinuxInputReader();
            }

            throw new NotImplementedException("Missing input class for your operation system!");
        }
    }
}
