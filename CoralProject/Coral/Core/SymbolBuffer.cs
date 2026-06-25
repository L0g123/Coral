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

        public void AddText(string text, Color2 color, Vector2i pos)
        {
            for(int i = 0; i < text.Length; i++)
            {
                var prev = Grid[Math.Min(pos.X + i, Grid.GetLength(0)), pos.Y];
                Grid[Math.Min(pos.X + i, Grid.GetLength(0)), pos.Y] += new ConsoleSymbol(color, text[i]);
            }
        }

        public SymbolBuffer(Vector2i size) : this(size.X, size.Y) { }

        public ConsoleSymbol this[int x, int y]
        {
            get => Grid[x, y];
            set => Grid[x, y] = value;
        }

        public void FlushToConsole()
        {
            StringBuilder strb = new();
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0;  x < Width; x++ )
                {
                    strb.Append(Grid[x, y].ToString());
                }
            }
            Console.Write(strb.ToString());
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

        public void Clear() => Fill(new(new((0, 0, 0, 255), (0, 0, 0, 255))));
    }
}
