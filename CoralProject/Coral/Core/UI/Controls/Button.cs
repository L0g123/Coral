using Coral.Core.IO.Input;

namespace Coral.Core.UI.Controls
{
    public class Button : Frame
    {
        public string ButtonText { get; set; } = string.Empty;
        public Color2 ButtonTextColor { get; set; } = Color2.WhiteOnTransparent;
        public bool Hovered { get; protected set; }
        public event Action Clicked = delegate { };

        public Button(string buttonText, Color2? buttonTextColor = null)
        {
            ButtonText = buttonText;
            ButtonTextColor = buttonTextColor ?? Color2.WhiteOnTransparent;

            InputHandler.Input.EventReceived += @event =>
            {
                if (
                    @event is InputEvent.Mouse { Value: var ev }
                    && ev.Kind == MouseEventKind.Press
                    && Hovered)
                {
                    Clicked.Invoke();
                }
            };
        }

        public override void RenderControl()
        {
            base.RenderControl();

            var textWidth = ButtonText.Length;
            var textPos = SymbolSize / 2 - new Vector2i(textWidth / 2, 0);
            RenderBuffer.AddText(ButtonText, ButtonTextColor, textPos);
        }

        public override void Update()
        {
            base.Update();
            Hovered = Bounds.Contains(InputHandler.MousePosition);
        }
    }
}
