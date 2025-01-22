using System.Collections.Generic;
using System.Linq;
using Gui.Services;
using Microsoft.Xna.Framework;

namespace Gui.Elements
{
    public class ContainerElement : ClickableElement
    {
        private readonly List<DrawableElement> _elements = new();

        public ContainerElement(IGuiServices guiServices) : base(guiServices)
        {
        }

        public IReadOnlyCollection<DrawableElement> Elements => _elements;

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

        protected override IEnumerable<DrawableElement> GetChildren()
        {
            return _elements;
        }

        public void AddElement(DrawableElement element)
        {
            if (IsInitialized)
            {
                element.InitializeInternal();
            }
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