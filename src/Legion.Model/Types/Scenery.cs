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

        private readonly List<Building> _buildings = new();
        public IReadOnlyList<Building> Buildings => _buildings;

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

        public Obstacle AddTemporaryObstacle(int x, int y, BoundingBox box)
        {
            var obstacle = new Obstacle(x, y, box, isTemporary: true);
            _obstacles.Add(obstacle);
            return obstacle;
        }

        public void RemoveTemporaryObstacles()
        {
            _obstacles.RemoveAll(o => o.IsTemporary);
        }

        public Building AddBuilding(Building building)
        {
            AddObstacle(building.X, building.Y, new BoundingBox(0, 0, building.Width, building.Height));
            if (building.DoorsPos > 0)
            {
                building.Box = new BoundingBox(building.DoorsPos, building.Height - 32, 32, 32);
            }
            _buildings.Add(building);
            return building;
        }
    }
}