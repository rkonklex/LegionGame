using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Gui.Input
{
    public class MouseEventArgs : HandledEventArgs
    {
        public MouseEventArgs(Point position)
        {
            Position = position;
        }

        public Point Position { get; private init; }
    }
}