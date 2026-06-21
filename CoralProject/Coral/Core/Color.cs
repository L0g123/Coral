using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    /// <summary>
    /// A simple container to hold RGBA color struct, normalized to [0,255] values
    /// </summary>
    public struct Color(int r, int g, int b, int a = 255) : IEquatable<Color>
    {
        public int R = r; public int G = g; public int B = b; public int A = a;

        public string ANSI => $"{r};{g};{b}m";
        public static string ANSIReset => "\x1B[0m";

        public static implicit operator Color((int r, int g, int b) t) => new(t.r, t.g, t.b, 255);
        public static implicit operator Color((int r, int g, int b, int a) t) => new(t.r, t.g, t.b, t.a);

        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }
    }
}
