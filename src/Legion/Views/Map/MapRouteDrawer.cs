using System;
using System.Diagnostics;
using Legion.Model.Types;
using Microsoft.Xna.Framework;

namespace Legion.Views.Map
{
    public class MapRouteDrawer : IMapRouteDrawer
    {
        public bool IsRouteDrawingForAny => IsRouteDrawingForPoint || IsRouteDrawingForMapObject;

        public bool IsRouteDrawingForPoint { get; private set; }

        public bool IsRouteDrawingForMapObject { get; private set; }

        public MapObject DrawingRouteSource { get; private set; }

        public event Action RouteDrawingStarted;

        public event Action RouteDrawingEnded;

        private Action<MapObject, Point> DrawingRouteForPointEnded { get; set; }

        private Action<MapObject, MapObject> DrawingRouteForMapObjectEnded { get; set; }

        public void StartRouteDrawingForPoint(MapObject mapObject, Action<MapObject, Point> onDrawingEnded)
        {
            if (IsRouteDrawingForAny)
            {
                throw new InvalidOperationException("Route drawing is already in progress");
            }
            DrawingRouteSource = mapObject;
            DrawingRouteForPointEnded = onDrawingEnded;
            IsRouteDrawingForPoint = true;
            RouteDrawingStarted?.Invoke();
        }

        public void StartRouteDrawingForMapObject(MapObject mapObject, Action<MapObject, MapObject> onDrawingEnded)
        {
            if (IsRouteDrawingForAny)
            {
                throw new InvalidOperationException("Route drawing is already in progress");
            }
            DrawingRouteSource = mapObject;
            DrawingRouteForMapObjectEnded = onDrawingEnded;
            IsRouteDrawingForMapObject = true;
            RouteDrawingStarted?.Invoke();
        }

        public void EndRouteDrawingForPoint(Point point)
        {
            if (!IsRouteDrawingForPoint)
            {
                throw new InvalidOperationException("Route drawing is not started");
            }
            DrawingRouteForPointEnded?.Invoke(DrawingRouteSource, point);
            FinalizeRouteDrawing();
        }

        public void EndRouteDrawingForMapObject(MapObject mapObject)
        {
            if (!IsRouteDrawingForMapObject)
            {
                throw new InvalidOperationException("Route drawing is not started");
            }
            DrawingRouteForMapObjectEnded?.Invoke(DrawingRouteSource, mapObject);
            FinalizeRouteDrawing();
        }

        public void CancelRouteDrawing()
        {
            if (!IsRouteDrawingForAny)
            {
                throw new InvalidOperationException("Route drawing is not started");
            }
            FinalizeRouteDrawing();
        }

        private void FinalizeRouteDrawing()
        {
            DrawingRouteSource = null;
            IsRouteDrawingForPoint = false;
            IsRouteDrawingForMapObject = false;
            RouteDrawingEnded?.Invoke();
        }
    }
}