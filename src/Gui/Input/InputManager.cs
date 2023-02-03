using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gui.Input
{
    //TODO: Handle input in better way
    public static class InputManager
    {
        public static Matrix ScaleMatrix;
        // Store current and previous states for comparison. 
        private static MouseState _previousMouseState;
        private static MouseState _currentMouseState;

        // Some keyboard states for later use.
        private static KeyboardState _previousKeyboardState;
        private static KeyboardState _currentKeyboardState;

        // Update the states so that they contain the right data.
        public static void Update()
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
        }

        public static Rectangle GetMouseBounds()
        {
            var pos = GetMousePostion();
            // Return a 1x1 squre representing the mouse click's bounding box.
            return new Rectangle(pos.X, pos.Y, 1, 1);
        }

        public static Point GetMousePostion()
        {
            var position = _currentMouseState.Position;
            var vect = new Vector2(position.X, position.Y);
            Vector2 worldPosition = Vector2.Transform(vect, Matrix.Invert(ScaleMatrix));
            return new Point((int)worldPosition.X, (int)worldPosition.Y);
        }

        public static bool IsMouseButtonUp(MouseButton btn)
        {
            return GetMouseButtonState(_currentMouseState, btn) == ButtonState.Released;
        }

        public static bool WasMouseButtonUp(MouseButton btn)
        {
            return GetMouseButtonState(_previousMouseState, btn) == ButtonState.Released;
        }

        public static bool IsMouseButtonDown(MouseButton btn)
        {
            return GetMouseButtonState(_currentMouseState, btn) == ButtonState.Pressed;
        }

        public static bool WasMouseButtonDown(MouseButton btn)
        {
            return GetMouseButtonState(_previousMouseState, btn) == ButtonState.Pressed;
        }

        public static bool IsMouseButtonJustPressed(MouseButton btn)
        {
            return IsMouseButtonDown(btn) && WasMouseButtonUp(btn);
        }

        public static bool IsMouseButtonJustReleased(MouseButton btn)
        {
            return WasMouseButtonDown(btn) && IsMouseButtonUp(btn);
        }

        private static ButtonState GetMouseButtonState(MouseState mouseState, MouseButton btn)
        {
            return btn switch
            {
                MouseButton.Left => mouseState.LeftButton,
                MouseButton.Middle => mouseState.MiddleButton,
                MouseButton.Right => mouseState.RightButton,
                _ => throw new System.NotImplementedException(),
            };
        }

        // TODO: Keyboard input stuff goes here.

        public static bool GetIsKeyDown(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        public static bool GetIsKeyUp(Keys key)
        {
            return _currentKeyboardState.IsKeyUp(key);
        }
    }
}