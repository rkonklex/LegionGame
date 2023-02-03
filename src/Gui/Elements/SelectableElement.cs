using System;
using System.ComponentModel;
using Gui.Input;
using Gui.Services;

namespace Gui.Elements
{
    public class SelectableElement : ClickableElement
    {
        public SelectableElement(IGuiServices guiServices) : base(guiServices)
        {
        }
        
        public event Action<HandledEventArgs> MouseEnter;
        public event Action<HandledEventArgs> MouseLeave;

        public bool IsMouseOver { get; private set; }

        protected virtual bool OnMouseEnter()
        {
            var args = new HandledEventArgs();
            MouseEnter?.Invoke(args);
            return args.Handled;
        }

        protected virtual bool OnMouseLeave()
        {
            var args = new HandledEventArgs();
            MouseLeave?.Invoke(args);
            return args.Handled;
        }

        internal override bool UpdateInputInternal()
        {
            var handled = false;
            var currentPosition = InputManager.GetMousePostion();

            // TODO: update IsMouseOver on IsVisible/IsEnabled change
            var wasMouseInside = IsMouseOver;
            var isMouseInside = Bounds.Contains(currentPosition);

            if (isMouseInside && !wasMouseInside)
            {
                IsMouseOver = true;
                handled = OnMouseEnter();
            }
            else if (wasMouseInside && !isMouseInside)
            {
                IsMouseOver = false;
                handled = OnMouseLeave();
            }

            return handled || base.UpdateInputInternal();
        }
    }
}