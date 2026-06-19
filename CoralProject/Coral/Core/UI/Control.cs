using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.UI
{
    /// <summary>
    /// Base abstract class that represents a UI widget
    /// </summary>
    public abstract class Control
    {
        protected Control(Viewport vp)
        {
            RootViewport = vp;

            Name = $"<{GetType().Name}>";
            Position = LayoutUnit.Zero;
            Size = LayoutUnit.Full;
            Origin = Vector2.Zero;
            Visible = true;
        }

        // Root viewport used for relative positioning when there are no parent controls
        public Viewport RootViewport;

        public int ID { get; private set; }
        public string Name { get; set; } = string.Empty;

        // Positioning
        public LayoutUnit Size { get; set; }
        public LayoutUnit Position { get; set; }
        public Vector2 Origin { get; set; }
        public Rectangle Bounds
        {
            get
            {
                var size = PixelSize;
                var pos = AbsolutePosition;
                return new(new((int)pos.X, (int)pos.Y), new((int)size.X, (int)size.Y));
            }
        }

        // Hierarchy
        public Control? Parent;
        public readonly List<Control> Children;

        // Renering
        public bool Visible { get; set; } = true;

        protected Vector2i GetParentSize() => Parent?.PixelSize ?? RootViewport.Bounds.Size;
        public Vector2i PixelSize => Size.GetSize(GetParentSize());

        // PixelPosition is relative to the parent's PixelPosition
        public Vector2i PixelPosition => (Vector2i)(Position.GetSize(GetParentSize()) - PixelSize * Origin);

        // AbsolutePosition is relative to the RootViewport
        public Vector2i AbsolutePosition => (Parent?.AbsolutePosition ?? Vector2.Zero) + PixelPosition;

        public Control GetRoot()
        {
            Control r = this;
            while (r.Parent != null) { r = r.Parent; }
            return r;
        }

        public Control? Find(Predicate<Control> f)
        {
            foreach (var child in Children)
            {
                if (f(child)) return child;
                var r = child.Find(f);
                if (r != null) return r;
            }
            return null;
        }
    }
}
