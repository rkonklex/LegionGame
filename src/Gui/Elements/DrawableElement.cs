using Gui.Services;
using Microsoft.Xna.Framework;

namespace Gui.Elements
{
    public class DrawableElement
    {
        public DrawableElement(IGuiServices guiServices)
        {
            GuiServices = guiServices;
        }

        protected IGuiServices GuiServices { get; }

        public Rectangle Bounds { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;

        internal virtual void InitializeInternal()
        {
            Initialize();
        }

        internal virtual void UpdateInternal()
        {
            Update();
        }

        internal virtual void DrawInternal()
        {
            Draw();
        }

        public virtual void Initialize() { }
        public virtual void Update() { }
        public virtual void Draw() { }

        public virtual bool HitTest(Point position, out DrawableElement hitElement)
        {
            if (IsVisible && Bounds.Contains(position))
            {
                hitElement = this;
                return true;
            }

            hitElement = null;
            return false;
        }
    }
}