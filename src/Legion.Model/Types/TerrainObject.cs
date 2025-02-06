using Legion.Utils;

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
}