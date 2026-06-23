using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.UI
{
    public class UIOrchestrator : IRenderSource
    {
        public Control Root { get; set; }

        protected SymbolBuffer Buffer { get; set; }
        protected Viewport TargetViewport { get; set; }

        public UIOrchestrator(Control root)
        {
            Root = root;            
            TargetViewport = new Viewport(Console.WindowWidth, Console.WindowHeight);
            Root.RootViewport = TargetViewport;
            Buffer = new(TargetViewport.Bounds.Size);
        }

        protected void Render()
        {
            // generate the front buffer if its the first frame
            Buffer ??= new SymbolBuffer(TargetViewport.Bounds.Size);

            Root.Render();
            Root.RenderBuffer.BlitTo(Buffer);
        }

        public SymbolBuffer GetOutputBuffer()
        {
            Render(); // render to back buffer

            return Buffer;
        }

        public void Update() { }
    }
}
