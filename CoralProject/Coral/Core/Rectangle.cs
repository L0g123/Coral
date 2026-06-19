using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    public struct Rectangle(Vector2i pos, Vector2i size)
    {
        public Vector2i Position { get; set; } = pos;
        public Vector2i Size { get; set; } = size;

    }
}
