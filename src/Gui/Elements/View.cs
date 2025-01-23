using System;
using System.Collections.Generic;
using System.Linq;
using Gui.Input;
using Gui.Services;
using Microsoft.Xna.Framework;

namespace Gui.Elements
{
    public abstract class View
    {
        private readonly List<Layer> _layers = [];
        private readonly IGuiServices _guiServices;
        private bool _isVisible;

        public View(IGuiServices guiServices)
        {
            _guiServices = guiServices;
        }

        protected IReadOnlyCollection<Layer> Layers => _layers;

        public void AddLayer(Layer layer)
        {
            if (layer.Parent is not null && layer.Parent != this)
            {
                throw new InvalidOperationException("Layer already has a different parent");
            }
            if (!_layers.Contains(layer))
            {
                _layers.Add(layer);
                layer.Parent = this;
            }
        }

        public void RemoveLayer(Layer layer)
        {
            if (layer.Parent is not null && layer.Parent != this)
            {
                throw new InvalidOperationException("Layer does not belong to this view");
            }
            if (_layers.Contains(layer))
            {
                _layers.Remove(layer);
                layer.Parent = null;
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                var isChanged = _isVisible != value;
                _isVisible = value;
                if (isChanged)
                {
                    if (_isVisible)
                    {
                        OnShowInternal();
                    }
                    else
                    {
                        OnHideInternal();
                    }
                }
            }
        }

        public object Context { get; set; }

        public virtual void Initialize() { } 
        public virtual void Update() { }
        public virtual void Draw() { }
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }

        internal void InitializeInternal()
        {
            Initialize();

            foreach (var layer in Layers)
            {
                layer.InitializeInternal();
            }
        }

        internal void UpdateInternal()
        {
            if (!IsVisible) { return; }

            Update();

            if (!IsVisible) { return; }

            // Make a snapshot to protect against list modification during the update loop
            var updateables = Layers.Where(la => la.IsVisible && la.IsEnabled).ToArray();

            var topMostModal = updateables.LastOrDefault(la => la.IsModal);
            if (topMostModal != null)
            {
                topMostModal.UpdateInputInternal();
                topMostModal.UpdateInternal();
            }
            else
            {
                foreach (var layer in updateables.Reverse())
                {
                    if (layer.UpdateInputInternal())
                    {
                        break;
                    }
                }

                foreach (var layer in updateables)
                {
                    layer.UpdateInternal();
                }
            }
        }

        internal void DrawInternal()
        {
            if (!IsVisible) { return; }

            Draw();

            var drawables = Layers.Where(la => la.IsVisible);
            foreach (var layer in drawables)
            {
                layer.DrawInternal();
            }
        }

        internal void OnShowInternal()
        {
            OnShow();

            foreach (var layer in Layers)
            {
                layer.OnShow();
            }
        }

        internal void OnHideInternal()
        {
            OnHide();

            foreach (var layer in Layers)
            {
                layer.OnHide();
            }
        }

        public bool HitTest(Point position, out DrawableElement hitElement, Layer belowLayer = null)
        {
            var layers = Layers.AsEnumerable();
            if (belowLayer is not null)
            {
                layers = layers.TakeWhile(la => la != belowLayer);
            }
            foreach (var layer in layers.Where(la => la.IsVisible).Reverse())
            {
                if (layer.HitTest(position, out hitElement))
                {
                    return true;
                }
            }

            hitElement = null;
            return false;
        }
    }
}