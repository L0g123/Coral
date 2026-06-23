using Coral.Core;
using Coral.Core.UI;
using Coral.Core.UI.Controls;

namespace CoralApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var frame = new Frame()
            {
                Size = LayoutUnit.Full,
                Position = LayoutUnit.Zero,
                Origin = new Vector2(0, 0),
                FrameBrush = new SolidBrush(new(new((255, 0, 0), (255, 0, 0))))
            };

            frame.AddChild(new Frame()
            {
                Size = LayoutUnit.Full,
                Position = new(0,0,.5f,.5f),
                Origin = new Vector2(0, 0),
                FrameBrush = new SolidBrush(new(new((0, 255, 0), (0, 0, 0)), 'c'))
            });

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
