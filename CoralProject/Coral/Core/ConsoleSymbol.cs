using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    public struct ConsoleSymbol(Color2 color, char character = ' ') : IEquatable<ConsoleSymbol>
    {
        public Color2 Color = color;
        public char Character = character;

        public bool Equals(ConsoleSymbol other)
        {
            return Color.Equals(other.Color) && Character == other.Character;
        }

        public override string ToString() => $"{Color}{Character}";

        public static ConsoleSymbol operator + (ConsoleSymbol left, ConsoleSymbol right)
        {
            var symbol = right.Color.Foreground!.Value.A == 0 ? left.Character : right.Character;
            return new(left.Color + right.Color, symbol);
        }

    }
}
