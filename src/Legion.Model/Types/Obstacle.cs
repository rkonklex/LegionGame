using Legion.Utils;

namespace Legion.Model.Types
{
    public class Obstacle : TerrainObject
    {
        public bool IsTemporary { get; init; }

        public Obstacle(int x, int y, BoundingBox box, bool isTemporary = false)
        {
            X = x;
            Y = y;
            Box = box;
            IsTemporary = isTemporary;
        }
    }
}