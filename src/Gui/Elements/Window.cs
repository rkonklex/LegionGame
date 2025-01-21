using System;
using System.ComponentModel;
using Gui.Services;

namespace Gui.Elements
{
    public class Window : Layer
    {
        public Window(IGuiServices guiServices) : base(guiServices)
        {
            IsModal = true;
        }

        public void Open(View parent)
        {
            Parent = parent;
            IsVisible = true;
        }

        public void Close()
        {
            IsVisible = false;
            Parent = null;
        }

        public override void Draw()
        {
            GuiServices.BasicDrawer.DrawRectangle(Colors.WindowBackgroundColor, Bounds);
            GuiServices.BasicDrawer.DrawBorder(Colors.WindowBorderColor, Bounds);
        }
    }
}