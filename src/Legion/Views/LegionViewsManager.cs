using System;
using System.Collections.Generic;
using Gui.Elements;
using Gui.Services;
using Legion.Model;
using Legion.Views.Map;
using Legion.Views.Menu;
using Legion.Views.Terrain;

namespace Legion.Views
{
    public class LegionViewsManager : ViewsManager, ILegionViewsManager
    {
        private readonly Lazy<MenuView> _menuView;
        private readonly Lazy<MapView> _mapView;
        private readonly Lazy<TerrainView> _terrainView;
        private List<View> _views;

        public LegionViewsManager(Lazy<MenuView> menuView, Lazy<MapView> mapView, Lazy<TerrainView> terrainView)
        {
            _menuView = menuView;
            _mapView = mapView;
            _terrainView = terrainView;
        }

        protected override List<View> Views
        {
            get
            {
                if (_views is null)
                {
                    _views = [_menuView.Value, _mapView.Value, _terrainView.Value];
                }
                return _views;
            }
            set
            {
                _views = value;
            }
        }

        public void OpenMenu()
        {
            CurrentView = _menuView.Value;
        }

        public void OpenMap()
        {
            CurrentView = _mapView.Value;
        }

        public void OpenTerrain(TerrainActionContext context)
        {
            var terrainView = _terrainView.Value;
            terrainView.Context = context;
            CurrentView = terrainView;
        }
    }
}