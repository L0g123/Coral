using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.UI.Controls
{
    public class Window(string title = "", Brush? brush = null) : Control
    {
        public Brush WindowBrush { get; set; } = brush ?? new BoxBorderBrush(Color2.FromForeground((255,255,255)));
        public string WindowTitle { get; set; } = title;
        public Color TitleColor { get; set; } = (255, 255, 255);

        public override void RenderControl()
        {
            RenderBuffer.Clear();
            for (int y = 0; y < RenderBuffer.Height; y++)
            {
                for (int x = 0; x < RenderBuffer.Width; x++)
                {
                    if(x == 0 || y == 0 || x == RenderBuffer.Width - 1 || y == RenderBuffer.Height - 1)
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
    }
}
