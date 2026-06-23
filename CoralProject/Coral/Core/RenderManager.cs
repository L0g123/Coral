using Coral.Core.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    /// <summary>
    /// Encapsulates different <see cref="IRenderSource">render sources</see>, and manages their sequential rendering to console and
    /// updating cycles
    /// </summary>
    /// <remarks>
    /// Requires render sources to output same-size buffers
    /// </remarks>
    public class RenderManager
    {
        protected SymbolBuffer FrontBuffer;
        protected SymbolBuffer BackBuffer;

        public readonly List<IRenderSource> RenderSources = [];

        public RenderManager(Viewport vp)
        {
            FrontBuffer = new(vp.Bounds.Size);
            BackBuffer = new(vp.Bounds.Size);
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

        public void Update()
        {
            BackBuffer?.Clear();
            foreach(var source in RenderSources)
            {
                var buf = source.GetOutputBuffer();

                if(BackBuffer == null)
                {
                    BackBuffer = new(buf.Size);
                    FrontBuffer = new(buf.Size);
                }

                if (!buf.Size.Equals(BackBuffer.Size))
                    throw new Exception($"SymbolBuffer size mismatch. ${source.GetType().Name} returned {buf.Size}; {BackBuffer.Size} expected.");

                buf.BlitTo(BackBuffer);
            }

            // switch buffers
            (FrontBuffer, BackBuffer) = (BackBuffer, FrontBuffer);
        }

        public void RenderToConsole()
        {
            var deltaSpans = CompressDelta(GetBufferDelta(BackBuffer, FrontBuffer), BackBuffer.Size.X);
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
        }
    }
}
