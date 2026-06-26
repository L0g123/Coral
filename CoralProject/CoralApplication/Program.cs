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

            frame.AddChild(new SubConsole()
            {
                Size = new(0, 0, .5f, .5f),
                Position = new(0, 0, .5f, .5f),
                Origin = new Vector2(.5f, .5f),
                Name = "bob",
            });

            // create UI orchestrator and add it to RenderManager

            UIOrchestrator orchestrator = new(frame);
            var manager = new RenderManager(Viewport.ConsoleViewport);
            manager.RenderSources.Add(orchestrator);

            // Create input handler
            var ihandler = InputReaderFactory.Create();
            var vconsole = frame.Find<SubConsole>("bob")!;
            ihandler.EventReceived += e =>
            {
                switch (e)
                {
                    case InputEvent.Mouse { Value: var m }:
                        vconsole.Out.WriteLine($"[Mouse] {m.Kind} {m.Button} @ ({m.Col}, {m.Row}) [{m.Modifiers}]");
                        break;

                    case InputEvent.Key { Value: var k } when k.IsDown:
                        vconsole.Out.WriteLine($"[Key]   {k.Key} '{k.Character}' [{k.Modifiers}]");
                        break;

                    case InputEvent.Resize { Value: var r }:
                        vconsole.Out.WriteLine($"[Resize] {r.Width}x{r.Height}");
                        break;
                }
            };

            while(true)
            {
                manager.Update();
                manager.RenderToConsole();
                
            }
        }
    }
}
