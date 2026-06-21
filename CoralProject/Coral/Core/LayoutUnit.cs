using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    public struct LayoutUnit(Vector2i abs, Vector2 rel)
    {
        public Vector2i Absolute { get; set; } = abs;
        public Vector2 Relative { get; set; } = rel;

        public readonly Vector2i Normalize(Vector2i measure) => (Vector2i)(measure * Relative) + Absolute;

        public LayoutUnit(int ax, int ay, float rx, float ry) : this(new(ax, ay), new(rx, ry)) { }

        public static LayoutUnit FromRelative(float rx, float ry) => new(0,0,rx,ry);
        public static LayoutUnit FromAbsolute(int ax, int ay) => new(ax, ay, 0, 0);

        public static LayoutUnit Full => new(0, 0, 1, 1);
        public static LayoutUnit Zero => new(0, 0, 0, 0);
    }
}
