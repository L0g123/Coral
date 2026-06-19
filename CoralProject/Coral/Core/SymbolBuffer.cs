using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    public class SymbolBuffer
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }

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

        public ConsoleSymbol this[int x, int y]
        {
            get => Grid[x, y];
            set => Grid[x, y] = value;
        }

        public void FlushTo(TextWriter writer)
        {
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
    }
}
