using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.UI.Controls
{
    internal class SubConsoleTextWriter(SubConsole console) : TextWriter
    {
        protected SubConsole Console = console;
        protected StringBuilder Buffer = new();

        public override void Flush()
        {
            var str = Buffer.ToString();
            Console.AddString(str);
            Buffer.Clear();
        }

        public override void Write(char value)
        {
            if (value == '\r') return;
            if(value == '\n') { Flush(); return; }
            Buffer.Append(value);
        }

        public override Encoding Encoding => Encoding.UTF8;
    }

    public class SubConsole : Window
    {
        public Color2 TextColor { get; set; } = Color2.WhiteOnTransparent;
        private SubConsoleTextWriter ConsoleStream { get; set; }
        public TextWriter Out { get => ConsoleStream; }

        protected List<string> ConsoleBuffer = [];
        public SubConsole() : base("SubConsole")
        {
            ConsoleStream = new(this);
        }

        public void AddString(string text) => ConsoleBuffer.Add(text);

        public override void RenderControl()
        {
            var maxTextLength = SymbolSize.X - 2;

            base.RenderControl(); // render basic window
            Vector2i targetPos = SymbolSize * new Vector2i(0, 1) + new Vector2i(1, -2);

            for(int strIdx = ConsoleBuffer.Count - 1;  strIdx >= 0; strIdx--)
            {
                var stackLength = ConsoleBuffer.Count - strIdx;
                if (stackLength > SymbolSize.Y - 2) break;

                var str = ConsoleBuffer[strIdx];
                int strLen = Math.Min(maxTextLength, str.Length);
                RenderBuffer.AddText(str[0..strLen], TextColor, targetPos);
                targetPos -= new Vector2i(0, 1);
            }

        }
    }
}
