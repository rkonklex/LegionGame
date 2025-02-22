using Legion.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Legion.Model.Types
{
    public abstract class TerrainObject
    {
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Bounding box, relative to the object's position.
        /// </summary>
        public BoundingBox Box { get; set; } = BoundingBox.Empty;

        /// <summary>
        /// Check if the object contains the given point.
        /// </summary>
        public bool BoxContains(int x, int y) => Box.Contains(x - X, y - Y);
    }

    public static class TerrainObjectExtensions
    {
        public static bool HitTest<TObject>(this IEnumerable<TObject> objects, int x, int y, out TObject hitObject)
            where TObject : TerrainObject
        {
            foreach (var obj in objects.Reverse())
            {
                if (obj.BoxContains(x, y))
                {
                    hitObject = obj;
                    return true;
                }
            }
            hitObject = null;
            return false;
        }

        public static int DistanceTo(this TerrainObject obj, int x, int y)
        {
            var dx = obj.X - x;
            var dy = obj.Y - y;
            return (int)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
