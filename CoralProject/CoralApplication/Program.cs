using Coral.Core;
using Coral.Core.UI;
using Coral.Core.UI.Controls;

namespace CoralApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UIOrchestrator orchestrator = new(new Frame()
            {
                Size = LayoutUnit.Full,
                Position = LayoutUnit.Zero,
                Origin = new Vector2(0, 0)
            });
            orchestrator.Render();
            orchestrator.FlushFrontBuffer();
        }
    }
}
