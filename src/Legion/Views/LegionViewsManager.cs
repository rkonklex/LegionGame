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
        private readonly MenuView _menuView;
        private readonly MapView _mapView;
        private readonly TerrainView _terrainView;

        public LegionViewsManager(MenuView menuView, MapView mapView, TerrainView terrainView)
        {
            _menuView = menuView;
            _mapView = mapView;
            _terrainView = terrainView;

            Views = new List<View> { menuView, mapView, terrainView };
        }

        protected override List<View> Views { get; set; }

        public void OpenMenu()
        {
            CurrentView = _menuView;
        }

        public void OpenMap()
        {
            CurrentView = _mapView;
        }

        public void OpenTerrain(TerrainActionContext context)
        {
            _terrainView.Context = context;
            CurrentView = _terrainView;
        }
    }
}