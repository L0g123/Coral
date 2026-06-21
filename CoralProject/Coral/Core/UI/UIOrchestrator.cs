using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.UI
{
    public class UIOrchestrator
    {
        public Control Root { get; set; }

        protected SymbolBuffer FrontBuffer { get; set; }
        protected SymbolBuffer BackBuffer { get; set; }
        protected Viewport TargetViewport { get; set; }

        public UIOrchestrator(Control root)
        {
            Root = root;            
            TargetViewport = new Viewport(Console.WindowWidth, Console.WindowHeight);
            Root.RootViewport = TargetViewport;
            FrontBuffer = new(TargetViewport.Bounds.Size);
            BackBuffer = new(TargetViewport.Bounds.Size);
        }

        public void Render()
        {
            // generate the front buffer if its the first frame
            FrontBuffer ??= new SymbolBuffer(TargetViewport.Bounds.Size);

            Root.Render();
            Root.RenderBuffer.BlitTo(BackBuffer);
        }

        protected static List<(Vector2i, ConsoleSymbol)> GetBufferDelta(SymbolBuffer prev, SymbolBuffer curr)
        {
            List<(Vector2i, ConsoleSymbol)> delta = [];

            for (int x = 0; x < prev.Width; x++)
            {
                for (int y = 0; y < prev.Height; y++)
                {
                    if (!prev[x, y].Equals(curr[x, y]))
                    {
                        delta.Add((new Vector2i(x, y), curr[x, y]));
                    }
                }
            }

            return delta;
        }


        public void FlushFrontBuffer()
        {
            foreach(var (pos, symbol) in GetBufferDelta(FrontBuffer, BackBuffer))
            {
                Console.SetCursorPosition(pos.X, pos.Y);
                Console.Write(symbol);
            }
        }
    }
}
