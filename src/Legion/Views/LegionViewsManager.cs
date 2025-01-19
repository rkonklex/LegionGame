using System;
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

        public LegionViewsManager(Lazy<MenuView> menuView, Lazy<MapView> mapView, Lazy<TerrainView> terrainView)
        {
            _menuView = menuView;
            _mapView = mapView;
            _terrainView = terrainView;
        }

        protected override void OnInitialize()
        {
            AddView(_menuView.Value);
            AddView(_mapView.Value);
            AddView(_terrainView.Value);
            OpenMenu();
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