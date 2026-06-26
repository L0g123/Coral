using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    /// <summary>
    /// Represents two colors simultaneously, labelled as a foreground and background colors
    /// </summary>
    public struct Color2(Color? fg, Color? bg) : IEquatable<Color2>
    {
        public Color? Foreground { get; set; } = fg;
        public Color? Background { get; set; } = bg;

        public Color2() : this((0, 0, 0), (0, 0, 0)) { }
        public Color2(Color col) : this(col, col) { }

        public override string ToString()
        {
            string result = string.Empty;
            if (Foreground != null) result += $"\x1B[38;2;{Foreground.Value.ANSI}";
            if (Background != null) result += $"\x1B[48;2;{Background.Value.ANSI}";

            return result;
        }

        public string ANSI => ToString();

        public readonly Color2 AsBackground => new((0, 0, 0, 0), Background!.Value);
        public readonly Color2 AsForeground => new(Foreground!.Value, (0, 0, 0, 0));

        public static Color2 FromBackground(Color bg) => new((0, 0, 0, 255), bg);
        public static Color2 FromForeground(Color fg) => new(fg, (0, 0, 0, 255));

        public bool Equals(Color2 other)
        {
            return Foreground.Equals(other.Foreground) && Background.Equals(other.Background);
        }

        public static Color2 operator +(Color2 left, Color2 right)
        {
            Color? fg =
                left.Foreground is null ? right.Foreground :
                right.Foreground is null ? left.Foreground :
                left.Foreground.Value + right.Foreground.Value;

            Color? bg =
                left.Background is null ? right.Background :
                right.Background is null ? left.Background :
                left.Background.Value + right.Background.Value;

            return new(fg, bg);
        }

        public static Color2 operator * (Color2 left, float right)
        {
            return new(left.Foreground * right, left.Background * right);
        }
        public static Color2 operator /(Color2 left, float right)
        {
            return new(left.Foreground / right, left.Background / right);
        }

        public static Color2 Transparent => new((0, 0, 0, 255), (0, 0, 0, 255));
        public static Color2 WhiteOnBlack => new((255, 255, 255), (0, 0, 0));
        public static Color2 WhiteOnTransparent => new((255, 255, 255), (0, 0, 0, 255));
    }
}
