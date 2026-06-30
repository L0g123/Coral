using Coral.Core.IO.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.UI.Controls
{
    public class Window : Control
    {
        public Brush WindowBrush { get; set; }
        public string WindowTitle { get; set; }
        public Color TitleColor { get; set; } = (255, 255, 255);

        public bool Draggable { get; set; } = false;
        protected bool IsHeld { get; set; } = false;
        protected Vector2i HandleOffset { get; set; } = Vector2i.Zero;

        public Window(string title = "", Brush? brush = null)
        {
            WindowBrush = brush ?? new BoxBorderBrush(Color2.FromForeground((255, 255, 255)));
            WindowTitle = title;

            InputHandler.Input.EventReceived += @event =>
            {
                if (Draggable && @event is InputEvent.Mouse { Value: var v })
                {
                    if (v.Kind == MouseEventKind.Press && v.Button == MouseButton.Left)
                    {
                        var mpos = InputHandler.MousePosition;
                        var wpos = AbsolutePosition;

                        if (mpos.Y == wpos.Y && mpos.X >= wpos.X && mpos.X <= wpos.X + SymbolSize.X)
                        {
                            IsHeld = true;
                            HandleOffset = mpos - wpos;
                        }
                    }
                    else if (v.Kind == MouseEventKind.Release && v.Button == MouseButton.Left)
                        IsHeld = false;
                }
            };
        }

        public override void RenderControl()
        {
            RenderBuffer.Clear();
            for (int y = 0; y < RenderBuffer.Height; y++)
            {
                for (int x = 0; x < RenderBuffer.Width; x++)
                {
                    if (x == 0 || y == 0 || x == RenderBuffer.Width - 1 || y == RenderBuffer.Height - 1)
                        RenderBuffer[x, y] += WindowBrush.GetSymbol(new(x, y), RenderBuffer.Size);
                }
            }

            if (WindowTitle != string.Empty)
            {
                var title = Util.Ellipsis(WindowTitle, (int)(SymbolSize.X / 2f));
                var textcolor = new Color2(TitleColor, (0, 0, 0, 255));
                RenderBuffer.AddText(title, textcolor, new(1, 0));
            }
        }

        public override void Update()
        {
            if (Draggable && IsHeld)
            {
                var pos = InputHandler.MousePosition - HandleOffset - (Vector2i)(SymbolSize * Origin);
                Position = Position with { Absolute = pos };
            }
        }
    }
}