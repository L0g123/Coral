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

            for (int y = 0; y < prev.Height; y++)
            {
                for (int x = 0; x < prev.Width; x++)
                {
                    if (!prev[x, y].Equals(curr[x, y]))
                    {
                        delta.Add((new Vector2i(x, y), curr[x, y]));
                    }
                }
            }

            return delta;
        }

        protected static List<(ConsoleSymbol symbol, Vector2i pos, int count)> CompressDelta(
            List<(Vector2i pos, ConsoleSymbol symbol)> delta,
            int bufferWidth
         )
        {
            var result = new List<(ConsoleSymbol symbol, Vector2i pos, int count)>();
            if (delta.Count == 0) return result;


            ConsoleSymbol spanSymbol = delta[0].symbol;
            Vector2i spanStart = delta[0].pos;
            int spanLinear = spanStart.Y * bufferWidth + spanStart.X;
            int spanCount = 1;

            for (int i = 1; i < delta.Count; i++)
            {
                Vector2i pos = delta[i].pos;
                ConsoleSymbol symbol = delta[i].symbol;
                int linear = pos.Y * bufferWidth + pos.X;

                if (linear == spanLinear + spanCount && spanSymbol.Color.Equals(symbol.Color))
                {
                    spanCount++;
                }
                else
                {
                    result.Add((spanSymbol, spanStart, spanCount));
                    spanSymbol = symbol;
                    spanStart = pos;
                    spanLinear = linear;
                    spanCount = 1;
                }
            }

            result.Add((spanSymbol, spanStart, spanCount));
            return result;
        }

        public void FlushFrontBuffer()
        {
            var deltaSpans = CompressDelta(GetBufferDelta(FrontBuffer, BackBuffer), TargetViewport.Bounds.Size.X);
            foreach (var (symbol, pos, count) in deltaSpans)
            {
                Console.SetCursorPosition(pos.X, pos.Y);
                StringBuilder str = new();
                for (int _ = 0; _ < count; _++)
                {
                    str.Append(symbol.ToString());
                }
                Console.Write(str.ToString());
            }

            // swap buffers
            (BackBuffer, FrontBuffer) = (FrontBuffer, BackBuffer);
        }
    }
}
