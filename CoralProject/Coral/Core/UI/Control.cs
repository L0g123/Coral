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
        protected Control()
        {
            Name = $"<{GetType().Name}>";
            Position = LayoutUnit.Zero;
            Size = LayoutUnit.Full;
            Origin = Vector2.Zero;
            Visible = true;
            //RenderBuffer = new(SymbolSize);
        }

        // Root viewport used for relative positioning when there are no parent controls
        public Viewport RootViewport { get; set; }

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
                var size = SymbolSize;
                var pos = AbsolutePosition;
                return new(new((int)pos.X, (int)pos.Y), new((int)size.X, (int)size.Y));
            }
        }

        // Hierarchy
        public Control? Parent;
        public readonly List<Control> Children = [];

        // Renering
        public bool Visible { get; set; } = true;
        public SymbolBuffer RenderBuffer { get; protected set; }

        protected Vector2i GetParentSize() => Parent?.SymbolSize ?? RootViewport.Bounds.Size;
        public Vector2i SymbolSize => Size.Normalize(GetParentSize());

        // SymbolPosition is relative to the parent's SymbolPosition
        public Vector2i SymbolPosition => (Vector2i)(Position.Normalize(GetParentSize()) - SymbolSize * Origin);

        // SymbolPosition is relative to the RootViewport
        public Vector2i AbsolutePosition => (Parent?.AbsolutePosition ?? Vector2.Zero) + SymbolPosition;

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
        public Control? Find(string name) => Find(c => c.Name == name);
        public T? Find<T>(Predicate<Control> f) where T : Control => Find(f) as T;
        public T? Find<T>(string name) where T : Control => Find(name) as T;
        protected void RegenerateRenderBuffer()
        {
            RenderBuffer = new(SymbolSize);
        }

        public override string ToString() => Name;

        // Render only the control itself, not its children
        public abstract void RenderControl();

        // Render the control and its children to the RenderBuffer
        public virtual void Render()
        {
            if (!Visible) return;

            RegenerateRenderBuffer();
            RenderControl();
            foreach (var child in Children)
            {
                child.Render();
                child.RenderBuffer.BlitTo(RenderBuffer, child.AbsolutePosition);
            }
        }

        public void AddChild(Control child)
        {
            if(child.Parent != null)
            {
                throw new InvalidOperationException("Child control already has a parent");
            }

            child.Parent = this;
            Children.Add(child);
        }
    }
}
