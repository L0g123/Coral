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
}
