using Gui.Services;
using Microsoft.Xna.Framework;

namespace Gui.Elements
{
    public class Button : Panel
    {
        public Button(IGuiServices guiServices, string text) : base(guiServices)
        {
            Text = text;
        }

        public bool Center { get; set; }

        public string Text { get; set; }

        public Color TextColor { get; set; } = Colors.TextDarkColor;

        protected override void OnPressedChanged()
        {
            Invert = IsPressed;
        }

        protected override void OnDraw()
        {
            base.OnDraw();

            int x = Center ? (Bounds.X + Bounds.Width / 2) : (Bounds.X + 4);
            int y = Bounds.Y + Bounds.Height / 2;
            GuiServices.BasicDrawer.DrawText(TextColor, x, y, Text, Center, true);
        }
    }
}