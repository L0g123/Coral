using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.UI.Controls
{
    public class Frame : Control
    {
        public ConsoleSymbol Symbol { get; set; } = new(new Color2((0,255,0), (0,255,0)), '█');

        public Frame() { }
        public override void RenderControl()
        {
            RenderBuffer.Fill(Symbol);
        }
    }
}
