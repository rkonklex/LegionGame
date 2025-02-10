using Legion.Utils;

namespace Legion.Model.Types
{
    public class Obstacle : TerrainObject
    {
        public Obstacle(int x, int y, BoundingBox box)
        {
            X = x;
            Y = y;
            Box = box;
        }
    }
}