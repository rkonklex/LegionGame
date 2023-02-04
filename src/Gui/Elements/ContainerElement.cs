using System.Collections.Generic;
using System.Linq;
using Gui.Services;

namespace Gui.Elements
{
    public class ContainerElement : ClickableElement
    {
        private readonly List<DrawableElement> _elements = new();

        public ContainerElement(IGuiServices guiServices) : base(guiServices)
        {
        }

        protected IReadOnlyCollection<DrawableElement> Elements => _elements;

        internal override bool UpdateInputInternal()
        {
            foreach (var elem in ((IEnumerable<DrawableElement>) Elements).Reverse())
            {
                if (elem is ClickableElement && elem.IsEnabled && elem.IsVisible)
                {
                    var handled = ((ClickableElement) elem).UpdateInputInternal();
                    if (handled) return true;
                }
            }
            return base.UpdateInputInternal();
        }

        internal override void UpdateInternal()
        {
            base.UpdateInternal();

            foreach (var elem in Elements)
            {
                if (elem.IsVisible) elem.UpdateInternal();
            }
        }

        internal override void DrawInternal()
        {
            base.DrawInternal();

            foreach (var elem in Elements)
            {
                if (elem.IsVisible) elem.DrawInternal();
            }
        }

        public void AddElement(DrawableElement element)
        {
            _elements.Add(element);
        }

        public void RemoveElement(DrawableElement element)
        {
            _elements.Remove(element);
        }

        public void ClearElements()
        {
            _elements.Clear();
        }
    }
}