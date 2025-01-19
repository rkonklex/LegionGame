using System;
using System.Collections.Generic;
using Gui.Elements;
using Gui.Input;

namespace Gui.Services
{
    public abstract class ViewsManager : IViewsManager
    {
        private readonly List<View> _views = new();
        protected IReadOnlyCollection<View> Views => _views;

        protected void AddView(View view)
        {
            _views.Add(view);
        }

        private View _currentView;
        public View CurrentView
        {
            get => _currentView;
            set
            {
                if (_currentView != value)
                {
                    foreach (var view in Views)
                    {
                        view.IsVisible = false;
                    }
                    _currentView = value;
                    _currentView.IsVisible = true;
                }
            }
        }

        public void Initialize()
        {
            OnInitialize();
            foreach (var view in Views)
            {
                view.InitializeInternal();
            }
        }

        protected virtual void OnInitialize()
        {
        }

        public void Update()
        {
            InputManager.Update();
            CurrentView.UpdateInternal();
        }

        public void Draw()
        {
            CurrentView.DrawInternal();
        }
    }
}