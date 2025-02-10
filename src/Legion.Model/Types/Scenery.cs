using Legion.Utils;
using System.Collections.Generic;

namespace Legion.Model.Types
{
    public class Scenery
    {
        public SceneryType Type { get; init; }

        private readonly List<Decoration> _decorations = new();
        public IReadOnlyList<Decoration> Decorations => _decorations;

        private readonly List<Obstacle> _obstacles = new();
        public IReadOnlyList<Obstacle> Obstacles => _obstacles;

        public Scenery(SceneryType sceneryType)
        {
            Type = sceneryType;
        }

        public Decoration AddDecoration(int x, int y, int bob, bool hrev = false)
        {
            var decoration = new Decoration(x, y, bob, hrev);
            _decorations.Add(decoration);
            return decoration;
        }

        public Obstacle AddObstacle(int x, int y, BoundingBox box)
        {
            var obstacle = new Obstacle(x, y, box);
            _obstacles.Add(obstacle);
            return obstacle;
        }
    }
}