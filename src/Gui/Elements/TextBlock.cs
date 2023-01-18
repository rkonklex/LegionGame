using Gui.Services;
using Microsoft.Xna.Framework;

namespace Gui.Elements
{
    public class TextBlock : ClickableElement
    {
        public TextBlock(IGuiServices guiServices) : base(guiServices)
        {
            Text = "";
            TextColor = Colors.TextDarkColor;
        }

        public bool IsHorizontalCenter { get; set; }

        public string Text { get; set; }

        public Color TextColor { get; set; }

        public override void Draw()
        {
            base.Draw();

            var textX = IsHorizontalCenter ? (Bounds.X + Bounds.Width / 2) : Bounds.X;
            var textY = Bounds.Y;
            var basicDrawer = GuiServices.BasicDrawer;
            // TODO: cache the result of LayoutTextLines()
            foreach (var line in basicDrawer.LayoutTextLines(Text, Bounds.Width))
            {
                basicDrawer.DrawText(TextColor, textX, textY, line,
                                     centerHorizontally: IsHorizontalCenter,
                                     centerVertically: false);
                textY += 10;
            }
        }
    }
}