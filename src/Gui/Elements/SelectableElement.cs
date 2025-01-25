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

        public event Action<MouseEventArgs> MouseEnter;
        public event Action<MouseEventArgs> MouseLeave;

        public bool IsMouseOver { get; private set; }

        protected internal override void OnMouseEnter(MouseEventArgs args)
        {
            base.OnMouseEnter(args);
            IsMouseOver = true;
            MouseEnter?.Invoke(args);
        }

        protected internal override void OnMouseLeave(MouseEventArgs args)
        {
            base.OnMouseLeave(args);
            IsMouseOver = false;
            MouseLeave?.Invoke(args);
        }
    }
}