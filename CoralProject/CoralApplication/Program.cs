using Coral.Core;

namespace CoralApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SymbolBuffer buffer = new(10, 10);
            buffer.Fill(new(new(null, (0, 255, 0)), 'c'));

            buffer.FlushTo(Console.Out);
        }
    }
}
