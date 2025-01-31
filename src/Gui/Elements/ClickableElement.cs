using System;
using System.ComponentModel;
using Gui.Input;
using Gui.Services;

namespace Gui.Elements
{
    public class ClickableElement : InputElement
    {
        public ClickableElement(IGuiServices guiServices) : base(guiServices) { }

        private bool _isPressed;
        public bool IsPressed
        {
            get => _isPressed;
            private set
            {
                if (_isPressed != value)
                {
                    _isPressed = value;
                    OnPressedChanged();
                }
            }
        }

        protected virtual void OnPressedChanged()
        {
        }

        public event Action<HandledEventArgs> Clicked;

        protected virtual void OnClick(HandledEventArgs args)
        {
            Clicked?.Invoke(args);
        }

        protected internal override void OnMouseDown(MouseButtonEventArgs args)
        {
            base.OnMouseDown(args);

            if (args.Button == MouseButton.Left)
            {
                args.Handled = true;
                IsPressed = true;
            }
        }

        protected internal override void OnMouseUp(MouseButtonEventArgs args)
        {
            base.OnMouseUp(args);

            if (args.Button == MouseButton.Left)
            {
                args.Handled = true;

                if (IsPressed)
                {
                    IsPressed = false;
                    OnClick(new HandledEventArgs());
                }
            }
        }

        protected internal override void OnMouseLeave(MouseEventArgs args)
        {
            base.OnMouseLeave(args);
            IsPressed = false;
        }
    }
}