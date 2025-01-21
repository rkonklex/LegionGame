using System.ComponentModel;
using System.Diagnostics;
using Gui.Input;
using Gui.Services;
using Legion.Model.Types;
using Legion.Views.Map.Controls;
using Microsoft.Xna.Framework;

namespace Legion.Views.Map.Layers
{
    public class DrawingLayer : MapLayer
    {
        private readonly IMapRouteDrawer _routeDrawer;
        private static readonly Color LineColor = Color.AntiqueWhite;

        public DrawingLayer(IGuiServices guiServices, IMapRouteDrawer routeDrawer) : base(guiServices)
        {
            IsModal = true;
            IsVisible = false;

            _routeDrawer = routeDrawer;
            _routeDrawer.RouteDrawingStarted += () => IsVisible = true;
            _routeDrawer.RouteDrawingEnded += () => IsVisible = false;
        }

        protected override bool OnMouseDown(MouseButton button, Point position)
        {
            Debug.Assert(_routeDrawer.IsRouteDrawingForAny);

            if (button == MouseButton.Left)
            {
                if (_routeDrawer.IsRouteDrawingForPoint)
                {
                    var mousePos = InputManager.GetMousePostion();
                    _routeDrawer.EndRouteDrawingForPoint(mousePos);
                    return true;
                }
                else if (Parent.HitTest(position, out var hitElement))
                {
                    MapObject mapObject = hitElement switch
                    {
                        ArmyElement armyElement => armyElement.Army,
                        CityElement cityElement => cityElement.City,
                        _ => null,
                    };
                    if (mapObject is not null)
                    {
                        _routeDrawer.EndRouteDrawingForMapObject(mapObject);
                        return true;
                    }
                }
            }
            else if (button == MouseButton.Right)
            {
                _routeDrawer.CancelRouteDrawing();
                return true;
            }

            return false;
        }

        protected override void OnDraw()
        {
            Debug.Assert(_routeDrawer.IsRouteDrawingForAny);

            var mapObject = _routeDrawer.DrawingRouteSource;
            var mousePos = InputManager.GetMousePostion();

            GuiServices.BasicDrawer.DrawLine(LineColor, 
                new Vector2(mapObject.X, mapObject.Y),
                mousePos.ToVector2());
        }
    }
}