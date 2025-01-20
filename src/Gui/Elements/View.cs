using System.Collections.Generic;
using System.Linq;
using Gui.Input;
using Gui.Services;

namespace Gui.Elements
{
    public abstract class View
    {
        private readonly IGuiServices _guiServices;
        private bool _isVisible;

        public View(IGuiServices guiServices)
        {
            _guiServices = guiServices;
        }

        protected abstract IEnumerable<Layer> Layers { get; }

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
                layer.Parent = this;
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
    }
}