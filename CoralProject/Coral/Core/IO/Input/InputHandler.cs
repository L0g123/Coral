namespace Coral.Core.IO.Input
{

    public static class InputHandler
    {
        public static readonly IInputReader Input = InputReaderFactory.Create();

        public static Vector2i MousePosition { get; private set; } = Vector2i.Zero;
        public static Vector2i MouseDelta { get; private set;  } = Vector2i.Zero;
        public static readonly Dictionary<ConsoleKey, bool> KeyboardState = [];
        public static readonly Dictionary<MouseButton, bool> MouseState = new()
        {
            {MouseButton.Left, false},
            {MouseButton.Right, false},
            {MouseButton.Middle, false},
        };

        static InputHandler()
        {
            Console.CursorVisible = false;

            foreach (var enumMember in Enum.GetValues<ConsoleKey>())
            {
                KeyboardState[enumMember] = false;
            }

            // basic handling for IsKeyDown() and IsMouseButtonDown()
            Input.EventReceived += @event =>
            {
                switch (@event)
                {
                    case InputEvent.Mouse { Value: var v }:
                        if (v.Kind == MouseEventKind.Press)
                        {
                            MouseState[v.Button] = true;
                        }
                        else if (v.Kind == MouseEventKind.Release)
                        {
                            MouseState[v.Button] = false;
                        }
                        else if (v.Kind == MouseEventKind.Move)
                        {
                            var newPos = new Vector2i(v.Col, v.Row);
                            MouseDelta = newPos - MousePosition;
                            MousePosition = newPos;
                        }
                        break;

                    case InputEvent.Key { Value: var v }:
                        KeyboardState[v.Key] = v.IsDown;
                        break;
                }
            };
        }

        public static bool IsKeyDown(ConsoleKey key) => KeyboardState[key];
        public static bool IsMouseButtonDown(MouseButton button) => MouseState[button];
    }
}
