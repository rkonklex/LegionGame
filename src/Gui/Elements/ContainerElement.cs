using System;
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

        protected override IEnumerable<DrawableElement> GetChildren()
        {
            return _elements;
        }

        public void AddElement(DrawableElement element)
        {
            if (element == this)
            {
                throw new InvalidOperationException("Element cannot be its own parent");
            }
            if (element.Parent is not null)
            {
                throw new InvalidOperationException("Element already has a parent");
            }

            if (IsInitialized)
            {
                element.InitializeInternal();
            }

            element.Parent = this;
            _elements.Add(element);
        }

        public void RemoveElement(DrawableElement element)
        {
            if (element.Parent != this)
            {
                throw new InvalidOperationException("Element does not belong to this container");
            }

            _elements.Remove(element);
            element.Parent = null;
        }

        public void ClearElements()
        {
            var elementsToRemove = _elements.ToArray();
            _elements.Clear();
            foreach (var element in elementsToRemove)
            {
                element.Parent = null;
            }
        }
    }
}