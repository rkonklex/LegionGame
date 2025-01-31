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

            var hittables = GetChildren().Where(c => c.IsVisible && c.IsEnabled && c.Contains(position));
            foreach (var elem in hittables.OfType<InputElement>().Reverse())
            {
                if (elem.InputHitTest(position, out hitElement))
                {
                    return true;
                }
            }

            if (IsHitTarget())
            {
                hitElement = this;
                return true;
            }

            hitElement = null;
            return false;
        }
    }
}