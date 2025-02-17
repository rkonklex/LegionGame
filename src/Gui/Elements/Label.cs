using Gui.Services;
using Microsoft.Xna.Framework;

namespace Gui.Elements
{
    public class Label : ClickableElement
    {
        public Label(IGuiServices guiServices) : base(guiServices)
        {
            Text = "";
            TextColor = Colors.TextDarkColor;
        }

        public bool IsHorizontalCenter { get; set; }

        public bool IsVerticalCenter { get; set; }

        public string Text { get; set; }

        public Color TextColor { get; set; }

        protected override void OnDraw()
        {
            base.OnDraw();

            int x = IsHorizontalCenter ? (Bounds.X + Bounds.Width / 2) : Bounds.X;
            int y = IsVerticalCenter ? (Bounds.Y + Bounds.Height / 2) : Bounds.Y;

            GuiServices.BasicDrawer.DrawText(TextColor, x, y, Text, IsHorizontalCenter, IsVerticalCenter);
        }
    }
}