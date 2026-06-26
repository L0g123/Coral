using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    public struct Rectangle(Vector2i pos, Vector2i size)
    {
        public Vector2i Position { get; set; } = pos;
        public Vector2i Size { get; set; } = size;

        public readonly Vector2i Center => Position + Size / 2;

        public Rectangle(int x, int y, int width, int height) : this(new Vector2i(x, y), new Vector2i(width, height)) { }

        public readonly bool Contains(Vector2i pos)
        {
            return 
                Position.X <= pos.X && pos.X <= Position.X + Size.X &&
                Position.Y <= pos.Y && pos.Y <= Position.Y + Size.Y;
        }
    }
}
