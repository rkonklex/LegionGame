using System;
using System.Linq;
using Gui.Elements;
using Gui.Services;
using Legion.Controllers.Map;
using Legion.Model.Types;
using Legion.Views.Map.Controls;

namespace Legion.Views.Map.Layers
{
    public class MapServices
    {
        
    }
    public class ArmiesLayer : MapLayer
    {
        private readonly IMapController _mapController;
        private readonly IMapArmyGuiFactory _armyGuiFactory;
        private readonly IMapRouteDrawer _routeDrawer;
        private readonly ModalLayer _modalLayer;

        public ArmiesLayer(
            IGuiServices guiServices,
            IMapController mapController,
            IMapArmyGuiFactory armyGuiFactory,
            IMapRouteDrawer routeDrawer,
            ModalLayer modalLayer) : base(guiServices)
        {
            _mapController = mapController;
            _armyGuiFactory = armyGuiFactory;
            _routeDrawer = routeDrawer;
            _modalLayer = modalLayer;
        }

        public override void OnShow()
        {
            base.OnShow();

            ClearElements();

            foreach (var army in _mapController.Armies)
            {
                var element = new ArmyElement(GuiServices, army);
                element.Clicked += args =>
                {
                    if (_routeDrawer.IsRouteDrawingForMapObject)
                    {
                        _routeDrawer.EndRouteDrawingForMapObject(army);
                    }
                    else
                    {
                        var armyWindow = _armyGuiFactory.CreateArmyWindow(army);
                        _modalLayer.Window = armyWindow;
                    }
                    args.Handled = true;
                };
                AddElement(element);
            }
        }

        public override void Update()
        {
            base.Update();

            if (_mapController.IsProcessingTurn)
            {
                foreach (var armyElement in Elements.Cast<ArmyElement>())
                {
                    if (armyElement.Army.IsKilled)
                    {
                        RemoveElement(armyElement);
                    }
                }
            }
        }
    }
}