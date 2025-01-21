using System.Collections.Generic;
using Gui.Services;
using Legion.Localization;
using Legion.Model;
using Legion.Model.Types;
using Legion.Views.Common.Controls;
using Legion.Views.Common.Controls.Equipment;
using Legion.Views.Map.Controls;
using Legion.Views.Map.Layers;
using Microsoft.Xna.Framework.Graphics;

namespace Legion.Views.Map
{
    public class MapArmyGuiFactory : IMapArmyGuiFactory
    {
        private readonly IGuiServices _guiServices;
        private readonly ITexts _texts;
        private readonly ICommonMapGuiFactory _commonMapGuiFactory;
        private readonly IMapRouteDrawer _mapRouteDrawer;

        public MapArmyGuiFactory(
            IGuiServices guiServices,
            ITexts texts,
            ICommonMapGuiFactory commonMapGuiFactory,
            IMapRouteDrawer mapRouteDrawer)
        {
            _guiServices = guiServices;
            _texts = texts;
            _commonMapGuiFactory = commonMapGuiFactory;
            _mapRouteDrawer = mapRouteDrawer;
        }

        public ArmyWindow CreateArmyWindow(Army army)
        {
            var window = new ArmyWindow(_guiServices, _texts, army);

            if (army.IsUserControlled)
            {
                window.MoreClicked += args =>
                {
                    var ordersWindow = CreateArmyOrdersWindow(army);
                    ordersWindow.Open(window.Parent);

                    // TODO: implement all actions handling
                    ordersWindow.MoveClicked += moveArgs => HandleMoveClick(army, ArmyActions.Move);
                    ordersWindow.FastMoveClicked += moveArgs => HandleMoveClick(army, ArmyActions.FastMove);
                    ordersWindow.AttackClicked += moveArgs =>
                    {
                        _mapRouteDrawer.StartRouteDrawingForMapObject(army, (source, target) =>
                        {
                            ((Army) source).CurrentAction = ArmyActions.Attack;
                            ((Army) source).Target = target;
                        });
                    };
                    ordersWindow.HuntClicked += moveArgs => army.CurrentAction = ArmyActions.Hunting;
                    ordersWindow.CampClicked += moveArgs => army.CurrentAction = ArmyActions.Camping;
                    ordersWindow.EquipmentClicked += _args =>
                    {
                        var equipmentWindow = new EquipmentWindow(_guiServices, _texts) {Army = army};
                        equipmentWindow.Open(ordersWindow.Parent);
                    };
                };
            }
            else if (army.DaysToGetInfo > 0 && army.DaysToGetInfo < 100)
            {
                window.MoreClicked += args =>
                {
                    var buyInformationWindow = _commonMapGuiFactory.CreateBuyInformationWindow(army);
                    buyInformationWindow.Open(window.Parent);
                };
            }

            return window;
        }

        private void HandleMoveClick(Army army, ArmyActions action)
        {
            _mapRouteDrawer.StartRouteDrawingForPoint(army, (mapObject, point) =>
            {
                ((Army) mapObject).CurrentAction = action;
                ((Army) mapObject).Target = new MapPosition {X = point.X, Y = point.Y};
            });
        }

        public ArmyOrdersWindow CreateArmyOrdersWindow(Army army)
        {
            //TODO: provide correct informations to the constructor instaead of 2x false
            var window = new ArmyOrdersWindow(_guiServices, _texts, false, false);
            return window;
        }
    }
}