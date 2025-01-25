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

        protected override void OnMouseDown(MouseButtonEventArgs args)
        {
            Debug.Assert(_routeDrawer.IsRouteDrawingForAny);

            base.OnMouseDown(args);
            args.Handled = true;

            if (args.Button == MouseButton.Left)
            {
                if (_routeDrawer.IsRouteDrawingForPoint)
                {
                    _routeDrawer.EndRouteDrawingForPoint(args.Position);
                }
                else if (Parent.HitTest(args.Position, out var hitElement, belowLayer: this))
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
                    }
                }
            }
            else if (args.Button == MouseButton.Right)
            {
                _routeDrawer.CancelRouteDrawing();
            }
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