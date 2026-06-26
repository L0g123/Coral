using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    /// <summary>
    /// A simple container to hold RGBA color struct, normalized to [0,255] values
    /// </summary>
    public struct Color(int r, int g, int b, int a = 0) : IEquatable<Color>
    {
        public int R = r; public int G = g; public int B = b; public int A = a;

        public string ANSI => $"{R};{G};{B}m";
        public static string ANSIReset => "\x1B[0m";

        public static implicit operator Color((int r, int g, int b) t) => new(t.r, t.g, t.b, 0);
        public static implicit operator Color((int r, int g, int b, int a) t) => new(t.r, t.g, t.b, t.a);

        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public static Color operator + (Color left, Color right)
        {
            float t = 1f - right.A / 255f;
            return (
                MathCoral.Lerp(left.R, right.R, t),
                MathCoral.Lerp(left.G, right.G, t),
                MathCoral.Lerp(left.B, right.B, t),
                MathCoral.Lerp(left.A, right.A, t)
                );
        }

        public static Color operator * (Color left, float right)
        {
            return ((int)(left.R * right), (int)(left.G * right), (int)(left.B * right), left.A);
        }

        public static Color operator /(Color left, float right)
        {
            return ((int)(left.R / right), (int)(left.G / right), (int)(left.B / right), left.A);
        }
    }
}
