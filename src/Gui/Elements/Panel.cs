using Gui.Services;
using Microsoft.Xna.Framework;

namespace Gui.Elements
{
    public class Panel : ClickableElement
    {
        public Panel(IGuiServices guiServices) : base(guiServices)
        { }

        public bool Invert { get; set; }

        public Color FillColor { get; set; } = Colors.PanelFillColor;
        public Color DarkColor { get; set; } = Colors.PanelDarkColor;
        public Color LightColor { get; set; } = Colors.PanelLightColor;

        public override void Draw()
        {
            var (x, y, width, height) = Bounds;
            var topLeftColor = Invert ? DarkColor : LightColor;
            var bottomRightColor = Invert ? LightColor : DarkColor;

            GuiServices.BasicDrawer.DrawRectangle(bottomRightColor, x, y, width, height);
            GuiServices.BasicDrawer.DrawRectangle(topLeftColor, x, y, width - 1, height - 1);
            GuiServices.BasicDrawer.DrawRectangle(FillColor, x + 1, y + 1, width - 2, height - 2);
        }
    }
}