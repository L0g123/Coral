using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.UI
{
    public abstract class Brush
    {
        public abstract ConsoleSymbol GetSymbol(Vector2i target, Vector2i canvasSize);
    }

    public class SolidBrush(ConsoleSymbol symbol) : Brush
    {
        public ConsoleSymbol BrushSymbol { get; set; } = symbol;
        public override ConsoleSymbol GetSymbol(Vector2i target, Vector2i canvasSize) => BrushSymbol;
    }

    public class BoxBorderBrush(Color2 color) : Brush
    {
        public Color2 BorderColor = color;
        public override ConsoleSymbol GetSymbol(Vector2i target, Vector2i canvasSize)
        {
            if(target.X == 0)
            {
                if (target.Y == 0) return new(BorderColor, '┌');
                else if (target.Y == canvasSize.Y - 1) return new(BorderColor, '└');
                else return new(BorderColor, '│');
            } else if (target.X == canvasSize.X - 1)
            {
                if (target.Y == 0) return new(BorderColor, '┐');
                else if (target.Y == canvasSize.Y - 1) return new(BorderColor, '┘');
                else return new(BorderColor, '│');
            } else
            {
                if (target.Y == 0) return new(BorderColor, '─');
                else if (target.Y == canvasSize.Y - 1) return new(BorderColor, '─');
                else return new(Color2.Transparent, ' ');
            }
        }
    }
}
