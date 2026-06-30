using Coral.Core;
using Coral.Core.IO.Input;
using Coral.Core.UI;
using Coral.Core.UI.Controls;

namespace CoralApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Set up UI hierarchy

            var frame = new Frame()
            {
                Size = LayoutUnit.Full,
                Position = LayoutUnit.Zero,
                Origin = new Vector2(0, 0),
                FrameBrush = new SolidBrush(new(new((20, 20, 20), (20, 20, 20))))
            };

            frame.AddChild(new Window("draggable window")
            {
                Size = new(0, 0, .5f, .5f),
                Position = new(0, 0, .5f, .5f),
                Origin = new Vector2(.5f, .5f),
                Name = "bob",
                Draggable = true
            });

            // create UI orchestrator and add it to RenderManager

            UIOrchestrator orchestrator = new(frame);
            var manager = new RenderManager(Viewport.ConsoleViewport);
            manager.RenderSources.Add(orchestrator);

            while(true)
            {
                manager.Update();
                manager.RenderToConsole();
                
            }
        }
    }
}
