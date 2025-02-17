using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gui.Services
{
    public interface IBasicDrawer
    {
        void DrawLine(Color color, Vector2 start, Vector2 end);
        void DrawBorder(Color color, int x, int y, int width, int height);
        void DrawBorder(Color color, Rectangle rectangle);
        void DrawRectangle(Color color, Rectangle rectangle);
        void DrawRectangle(Color color, int x, int y, int width, int height);
        void DrawText(Color color, int x, int y, string text,
            bool centerHorizontally = false,
            bool centerVertically = false);
        Vector2 MeasureText(string text);
        string[] LayoutTextLines(string text, int maxWidth);
        void DrawImage(Texture2D image, float x, float y);
    }
}