using System;
using System.Diagnostics;
using System.Linq;
using Gui.Input;
using Gui.Services;
using Microsoft.Xna.Framework;

namespace Gui.Elements
{
    public class InputElement : DrawableElement
    {
        public InputElement(IGuiServices guiServices) : base(guiServices) { }

        public event Action<MouseButtonEventArgs> MouseDown;
        public event Action<MouseButtonEventArgs> MouseUp;

        protected internal virtual void OnMouseDown(MouseButtonEventArgs args)
        {
            MouseDown?.Invoke(args);
        }

        protected internal virtual void OnMouseUp(MouseButtonEventArgs args)
        {
            MouseUp?.Invoke(args);
        }

        protected internal virtual void OnMouseEnter(MouseEventArgs args)
        {
        }

        protected internal virtual void OnMouseLeave(MouseEventArgs args)
        {
        }

        internal bool InputHitTest(Point position, out InputElement hitElement)
        {
            Debug.Assert(IsVisible && IsEnabled);

            var hittables = GetChildren().OfType<InputElement>().Where(c => c.IsVisible && c.IsEnabled);
            foreach (var elem in hittables.Reverse())
            {
                if (elem.InputHitTest(position, out hitElement))
                {
                    return true;
                }
            }

            if (IsHit(position))
            {
                hitElement = this;
                return true;
            }

            hitElement = null;
            return false;
        }
    }
}