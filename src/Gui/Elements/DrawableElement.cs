using Gui.Services;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

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

        public Point Position
        {
            get => Bounds.Location;
            set => Bounds = new Rectangle(value, Bounds.Size);
        }

        public Point Size
        {
            get => Bounds.Size;
            set => Bounds = new Rectangle(Bounds.Location, value);
        }

        public DrawableElement Parent { get; internal set; }

        protected virtual IEnumerable<DrawableElement> GetChildren() => Enumerable.Empty<DrawableElement>();

        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;

        public bool IsInitialized { get; private set; } = false;

        internal void InitializeInternal()
        {
            if (!IsInitialized)
            {
                OnInitialize();
                foreach (var child in GetChildren())
                {
                    child.InitializeInternal();
                    child.Parent = this;
                }
                IsInitialized = true;
            }
        }

        internal void UpdateInternal()
        {
            OnUpdate();

            // Make a copy of the list to avoid modification during iteration
            var updateables = GetChildren().Where(c => c.IsVisible && c.IsEnabled).ToArray();
            foreach (var child in updateables)
            {
                child.UpdateInternal();
            }
        }

        internal void DrawInternal()
        {
            OnDraw();

            var drawables = GetChildren().Where(c => c.IsVisible);
            foreach (var child in drawables)
            {
                child.DrawInternal();
            }
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnDraw() { }

        public bool HitTest(Point position, out DrawableElement hitElement)
        {
            foreach (var elem in GetChildren().Where(c => c.IsVisible).Reverse())
            {
                if (elem.HitTest(position, out hitElement))
                {
                    return true;
                }
            }

            if (IsHit(position))
            {
                hitElement = this;
                return true;
            }

            hitElement = null;
            return false;
        }

        protected virtual bool IsHit(Point position)
        {
            return Bounds.Contains(position);
        }
    }
}