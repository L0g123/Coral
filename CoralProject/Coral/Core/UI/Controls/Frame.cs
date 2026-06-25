using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.UI.Controls
{
    public class Frame : Control
    {
        public Brush FrameBrush { get; set; } = new SolidBrush(new(new Color2((0, 255, 0), (0, 255, 0)), '█'));

        public Frame() { }
        public override void RenderControl()
        {
            for (int y = 0; y < RenderBuffer.Height; y++)
                for (int x = 0; x < RenderBuffer.Width; x++)
                    RenderBuffer[x, y] = FrameBrush.GetSymbol(new(x, y), RenderBuffer.Size);
        }
    }
}
