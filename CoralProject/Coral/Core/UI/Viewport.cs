using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.UI
{
    public class Viewport(int width, int height)
    {
        public Rectangle Bounds { get; } = new Rectangle(0, 0, width, height);

        public static Viewport ConsoleViewport => new Viewport(Console.WindowWidth, Console.WindowHeight);
    }
}
