using System.Collections.Generic;
using Gui.Elements;
using Gui.Services;
using Legion.Views.Map.Layers;

namespace Legion.Views.Map
{
    public class MapView : View
    {
        public MapView(IGuiServices guiServices,
            MapBackgroundLayer mapLayer,
            CitiesLayer citiesLayer,
            ArmiesLayer armiesLayer,
            MapGuiLayer mapGuiLayer,
            DrawingLayer drawingLayer) : base(guiServices)
        {
            AddLayer(mapLayer);
            AddLayer(citiesLayer);
            AddLayer(armiesLayer);
            AddLayer(mapGuiLayer);
            AddLayer(drawingLayer);
        }
    }
}