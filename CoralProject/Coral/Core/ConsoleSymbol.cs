using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    public struct ConsoleSymbol(Color2 color, char character = ' ')
    {
        public Color2 Color = color;
        public char Character = character;

        public override string ToString() => $"{Color}{Character}";

    }
}
