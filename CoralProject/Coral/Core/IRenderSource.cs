namespace Coral.Core
{
    /// <summary>
    /// Represents a state machine (ex. <see cref="UI.UIOrchestrator"/>) that is
    /// able to output finalized image buffer that is ready to be rendered to the console
    /// </summary>
    public interface IRenderSource
    {
        public SymbolBuffer GetOutputBuffer();
        public void Update();
    }
}
