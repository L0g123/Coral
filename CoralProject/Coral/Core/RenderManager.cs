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
            var delta = GetBufferDelta(BackBuffer, FrontBuffer);

            /*
             * Rendering via delta requires three payloads to console (cursor X, cursor Y, data)
             * Hence, we use a heuristic to determine when its better to just flush the entire
             * buffer sequentially
             */
            if(delta.Count * 1.5f > FrontBuffer.Width * FrontBuffer.Height)
            {
                FrontBuffer.FlushToConsole();
            } else
            {
                foreach(var (pos, sym) in delta)
                {
                    Console.SetCursorPosition(pos.X, pos.Y);
                    Console.Write(sym.ToString());
                }
            }
        }
    }
}
