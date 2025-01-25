using System;
using System.ComponentModel;
using Gui.Input;
using Gui.Services;

namespace Gui.Elements
{
    public class ClickableElement : InputElement
    {
        private bool _wasDown;

        public ClickableElement(IGuiServices guiServices) : base(guiServices) { }

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
                _wasDown = true;
            }
        }

        protected internal override void OnMouseUp(MouseButtonEventArgs args)
        {
            base.OnMouseUp(args);

            if (args.Button == MouseButton.Left)
            {
                args.Handled = true;

                if (_wasDown)
                {
                    _wasDown = false;
                    OnClick(new HandledEventArgs());
                }
            }
        }

        protected internal override void OnMouseLeave(MouseEventArgs args)
        {
            base.OnMouseLeave(args);
            _wasDown = false;
        }
    }
}