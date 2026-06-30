using Coral.Core.IO.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.UI.Controls
{
    public class ScrollWindow : Window
    {
        public bool Hovered { get; protected set; }

        protected int ScrollOffset
        {
            get;
            set
            {
                if (value > MaxScroll || value < MinScroll) return;

                var delta = new Vector2i(0, 1) * (field - value);
                field = value;
                foreach (var child in Children) child.Position = child.Position with { Absolute = child.Position.Absolute + delta };
            }
        }
        protected int MaxScroll
        {
            get
            {
                var lowest = Children.MaxBy(c => c.SymbolPosition.Y + c.SymbolSize.Y);
                if (lowest == null) return ScrollOffset;

                // SymbolPosition is already the top-left corner (Origin is baked in),
                // so the bottom edge is just position + size.
                var contentBottom = lowest.SymbolPosition.Y + lowest.SymbolSize.Y;

                // Children positions already reflect the current ScrollOffset, so the
                // limit has to be anchored to it rather than treated as an absolute value.
                return ScrollOffset + Math.Max(0, contentBottom - SymbolSize.Y);
            }
        }

        protected int MinScroll
        {
            get
            {
                var highest = Children.MinBy(c => c.SymbolPosition.Y);
                if (highest == null) return ScrollOffset;

                var contentTop = highest.SymbolPosition.Y;
                return ScrollOffset + Math.Min(0, contentTop);
            }
        }

        public ScrollWindow()
        {
            InputHandler.Input.EventReceived += @event =>
            {
                if (@event is InputEvent.Mouse { Value: var ev } && Hovered)
                {
                    switch (ev.Kind)
                    {
                        case MouseEventKind.ScrollUp:
                            ScrollOffset--;
                            break;
                        case MouseEventKind.ScrollDown:
                            ScrollOffset++;
                            break;
                    }
                }
            };
        }

        public override void Update()
        {
            base.Update();
            Hovered = Bounds.Contains(InputHandler.MousePosition);
        }
    }
}