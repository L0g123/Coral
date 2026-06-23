using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    public class SymbolBuffer
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public Vector2i Size => new(Width, Height);

        protected ConsoleSymbol[,] Grid;

        public SymbolBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            Grid = new ConsoleSymbol[Width, Height];
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Grid[i, j] = new();
                }
            }
        }

        public SymbolBuffer(Vector2i size) : this(size.X, size.Y) { }

        public ConsoleSymbol this[int x, int y]
        {
            get => Grid[x, y];
            set => Grid[x, y] = value;
        }

        public void FlushTo(TextWriter writer)
        {
            Console.SetCursorPosition(0, 0);
            for (int x = 0; x < Grid.GetLength(0); x++)
            {
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    writer.Write(Grid[x, y]);
                }
                writer.WriteLine();
            }
            writer.Flush();
        }

        public void Fill(ConsoleSymbol symbol)
        {
            for (int x = 0; x < Grid.GetLength(0); x++)
            {
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    Grid[x, y] = symbol;
                }
            }
        }

        public void BlitTo(SymbolBuffer symbolBuffer) => BlitTo(symbolBuffer, new Vector2i(0, 0));

        public void BlitTo(SymbolBuffer symbolBuffer, Vector2i position)
        {
            for (int x = 0; x < Math.Min(Width, symbolBuffer.Width - position.X); x++)
            {
                for (int y = 0; y < Math.Min(Height, symbolBuffer.Height - position.Y); y++)
                {
                    var result = symbolBuffer[x + position.X, y + position.Y] + Grid[x, y];
                    symbolBuffer[x + position.X, y + position.Y] = result;
                }
            }
        }

        public void Clear() => Fill(new(new((0, 0, 0, 0), (0, 0, 0, 0))));
    }
}
