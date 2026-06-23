using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    public struct Vector2i(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;

        public static Vector2i Zero => new(0, 0);
        public static Vector2i One => new(1, 1);
        public Vector2i(int v) : this(v, v) { }

        public static Vector2i operator +(Vector2i left, Vector2i right)
        {
            return new(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2i operator -(Vector2i left, Vector2i right)
        {
            return new(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2i operator *(Vector2i left, Vector2i right)
        {
            return new(left.X * right.X, left.Y * right.Y);
        }

        public static Vector2i operator /(Vector2i left, Vector2i right)
        {
            return new(left.X / right.X, left.Y / right.Y);
        }

        public static Vector2i operator *(Vector2i left, int right)
        {
            return new(left.X * right, left.Y * right);
        }

        public static Vector2i operator *(Vector2i left, float right)
        {
            return new((int)(left.X * right), (int)(left.Y * right));
        }

        public static Vector2i operator /(Vector2i left, int right)
        {
            return new(left.X / right, left.Y / right);
        }

        public static Vector2i operator /(Vector2i left, float right)
        {
            return new((int)(left.X / right), (int)(left.Y / right));
        }

        public static implicit operator Vector2(Vector2i v) => new(v.X, v.Y);

        public override string ToString() => $"({X}, {Y})";
    }
}
