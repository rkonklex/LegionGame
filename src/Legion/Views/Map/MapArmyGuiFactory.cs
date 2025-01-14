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
        private readonly ILegionConfig _legionConfig;
        private readonly ITexts _texts;
        private readonly ICommonMapGuiFactory _commonMapGuiFactory;
        private readonly IMapRouteDrawer _mapRouteDrawer;
        private readonly ModalLayer _modalLayer;
        private List<Texture2D> _armyWindowImages;

        public MapArmyGuiFactory(
            IGuiServices guiServices,
            ILegionConfig legionConfig,
            ITexts texts,
            ICommonMapGuiFactory commonMapGuiFactory,
            IMapRouteDrawer mapRouteDrawer,
            ModalLayer modalLayer)
        {
            _guiServices = guiServices;
            _legionConfig = legionConfig;
            _texts = texts;
            _commonMapGuiFactory = commonMapGuiFactory;
            _mapRouteDrawer = mapRouteDrawer;
            _modalLayer = modalLayer;

            guiServices.GameLoaded += LoadImages;
        }

        private void LoadImages()
        {
            _armyWindowImages = _guiServices.ImagesStore.GetImages("army.windowUsers");
        }

        public ArmyWindow CreateArmyWindow(Army army)
        {
            var window = new ArmyWindow(_guiServices);
            var hasData = false;
            var infoText = "";

            window.NameText = army.Name;
            window.Image = _armyWindowImages[army.Owner.Id - 1];

            window.ButtonOkText = _texts.Get("infoWindow.ok");
            if (army.Owner.IsUserControlled)
            {
                window.ButtonMoreText = _texts.Get("infoWindow.commands");
                hasData = true;
            }
            else
            {
                window.ButtonMoreText = _texts.Get("infoWindow.inquiry");
                if (army.DaysToGetInfo > 28 && army.DaysToGetInfo < 100)
                {
                    hasData = false;
                    infoText = _texts.Get("infoWindow.noInformation");
                }
                else
                {
                    infoText = army.DaysToGetInfo > 1 ?
                        _texts.Get("infoWindow.informationsInXDays", army.DaysToGetInfo) :
                        _texts.Get("infoWindow.informationsInOneDay");
                    hasData = false;
                }
                if (army.DaysToGetInfo == 0 || army.DaysToGetInfo == 100)
                {
                    hasData = true;
                    window.ButtonMoreText = _texts.Get("infoWindow.trace");
                }
            }

            if (!hasData && !_legionConfig.GoDmOdE)
            {
                window.InfoText = infoText;
            }
            else
            {
                var count = army.Characters.Count;
                window.CountText = count == 1 ?
                    _texts.Get("armyInfo.oneWarrior") :
                    _texts.Get("armyInfo.xWarriors", count);

                int foodCount = army.Food / army.Characters.Count;
                if (foodCount > 1) window.FoodText = _texts.Get("armyInfo.foodForXDays", foodCount);
                else if (foodCount == 1) window.FoodText = _texts.Get("armyInfo.foodForOneDay");
                else window.FoodText = _texts.Get("armyInfo.noMoreFood");

                window.StrengthText = _texts.Get("armyInfo.strength", army.Strength);
                window.SpeedText = _texts.Get("armyInfo.speed", army.Speed);

                window.ActionText = "";
                switch (army.CurrentAction)
                {
                    case ArmyActions.Camping:
                        window.ActionText = _texts.Get("armyInfo.camping");
                        /* TODO:
                         If TEREN>69
                            RO$=RO$+" w "+MIASTA$(TEREN-70)
                         End If 
                         */
                        break;
                    case ArmyActions.Move:
                    case ArmyActions.FastMove:
                        window.ActionText = _texts.Get("armyInfo.moving");
                        break;
                    case ArmyActions.Attack:
                        window.ActionText = _texts.Get("armyInfo.attackingX", army.Target.Name);
                        /* TODO:
                         If CELY=0
                            R2$=ARMIA$(CELX,0)
                         Else 
                            R2$=MIASTA$(CELX)
                         End If 
                         RO$="Atakujemy "+R2$
                        */
                        break;
                    case ArmyActions.Hunting:
                        window.ActionText = _texts.Get("armyInfo.hunting");
                        break;
                }
            }

            if (army.Owner.IsUserControlled)
            {
                window.MoreClicked += args =>
                {
                    var ordersWindow = CreateArmyOrdersWindow(army);
                    _modalLayer.Window = ordersWindow;

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
                        _modalLayer.Window = equipmentWindow;
                    };
                };
            }
            else if (army.DaysToGetInfo > 0 && army.DaysToGetInfo < 100)
            {
                window.MoreClicked += args =>
                {
                    _modalLayer.Window = _commonMapGuiFactory.CreateBuyInformationWindow(army);
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