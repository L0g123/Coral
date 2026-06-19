using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    public struct Vector2(float x, float y)
    {
        public float X { get; set; } = x;
        public float Y { get; set; } = y;

        public static Vector2i Zero => new(0, 0);
        public static Vector2i One => new(1, 1);

        public Vector2(int v) : this(v, v) { }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2 operator *(Vector2 left, Vector2 right)
        {
            return new(left.X * right.X, left.Y * right.Y);
        }

        public static Vector2 operator /(Vector2 left, Vector2 right)
        {
            return new(left.X / right.X, left.Y / right.Y);
        }

        public static Vector2 operator *(Vector2 left, float right)
        {
            return new(left.X * right, left.Y * right);
        }

        public static Vector2 operator /(Vector2 left, int right)
        {
            return new(left.X / right, left.Y / right);
        }

        public static Vector2 operator /(Vector2 left, float right)
        {
            return new(left.X / right, left.Y / right);
        }

        public static explicit operator Vector2i(Vector2 v) => new((int)v.X, (int)v.Y);
    }
}
