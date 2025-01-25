using Microsoft.Xna.Framework;

namespace Gui.Input
{
    public class MouseButtonEventArgs : MouseEventArgs
    {
        public MouseButtonEventArgs(MouseButton button, Point position): base(position)
        {
            Button = button;
        }

        public MouseButton Button { get; private init; }
    }
}