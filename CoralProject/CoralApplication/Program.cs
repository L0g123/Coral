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
                FrameBrush = new SolidBrush(new(new((255, 0, 0), (255, 0, 0))))
            };

            frame.AddChild(new Button("im a button")
            {
                Size = new(25, 5, 0, 0),
                Position = new(0, 0, .5f, .5f),
                Origin = new Vector2(.5f, .5f),
                Name = "bob",
                FrameBrush = new BoxBorderBrush(Color2.WhiteOnTransparent)
            });

            frame.Find<Button>("bob")!.Clicked += () => { frame.Find<Button>("bob")!.ButtonText = "clicked"; };

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
