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

        public ArmiesLayer(
            IGuiServices guiServices,
            IMapController mapController,
            IMapArmyGuiFactory armyGuiFactory) : base(guiServices)
        {
            _mapController = mapController;
            _armyGuiFactory = armyGuiFactory;
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
                    var armyWindow = _armyGuiFactory.CreateArmyWindow(army);
                    armyWindow.Open(Parent);
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
                var elementsToRemove = Elements.Cast<ArmyElement>().Where(e => e.Army.IsKilled).ToList();
                foreach (var element in elementsToRemove)
                {
                    RemoveElement(element);
                }
            }
        }
    }
}