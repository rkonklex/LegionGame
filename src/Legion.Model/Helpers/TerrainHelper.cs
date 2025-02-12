using Legion.Model.Helpers;
using Legion.Model.Types;
using Legion.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Legion.Model
{
    public class TerrainHelper : ITerrainHelper
    {
        public TerrainActionContextBuilder BuildTerrainActionContext()
        {
            return new TerrainActionContextBuilder(this);
        }

        public Scenery CreateScenery(SceneryType sceneryType, City city = null)
        {
            var scenery = new Scenery(sceneryType);
            var isInsideCity = city is not null;

            if (isInsideCity)
            {
                // building foundations (temporary, later replaced by actual buildings)
                foreach (var building in city.Buildings)
                {
                    var box = new BoundingBox(0, 0, building.Width, building.Height + 40);
                    scenery.AddTemporaryObstacle(building.X, building.Y, box);
                }
            }

            PopulateScenery(scenery, isInsideCity);

            scenery.RemoveTemporaryObstacles();

            if (isInsideCity)
            {
                foreach (var building in city.Buildings)
                {
                    scenery.AddBuilding(building);
                }
            }

            return scenery;
        }

        private static void PopulateScenery(Scenery scenery, bool isInsideCity)
        {
            var d = isInsideCity ? 2 : 1;

            switch (scenery.Type)
            {
                case SceneryType.Forest:
                    // decorations
                    for (int i = 0; i <= 30; i++)
                    {
                        int x = GlobalUtils.Rand(620) + 20;
                        int y = i * 20;
                        int bob = GlobalUtils.Rand(1) + 4;
                        scenery.AddDecoration(x, y, bob);
                    }

                    // small obstacles
                    for (int i = 0; i <= 15 / d; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var bob = GlobalUtils.Rand(2) + 1;
                        var hrev = GlobalUtils.Rand(1) == 0;
                        var box = new BoundingBox(4, 4, 24, 18);
                        scenery.AddDecoration(x, y, bob, hrev);
                        scenery.AddObstacle(x, y, box);
                    }

                    if (!isInsideCity)
                    {
                        // trees
                        for (int j = 0; j <= 3; j++)
                        {
                            var x2 = GlobalUtils.Rand(640);
                            var y2 = j * 100;
                            for (int i = 0; i <= 18; i++)
                            {
                                var x = x2 + GlobalUtils.Rand(100) - 50;
                                var y = y2 + (i * 4) - 60;
                                var bob = GlobalUtils.Rand(2) + 6;
                                scenery.AddDecoration(x, y, bob);
                            }
                            var box1 = new BoundingBox(-50, -60, 190, 130);
                            var box2 = new BoundingBox(-10, box1.Bottom, 105, 50);
                            scenery.AddObstacle(x2, y2, box1);
                            scenery.AddObstacle(x2, y2, box2);
                        }
                    }
                    break;

                case SceneryType.Steppe:
                    // decorations
                    for (int i = 1; i <= 20; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var bob = GlobalUtils.Rand(1) + 1;
                        var hrev = GlobalUtils.Rand(1) == 0;
                        scenery.AddDecoration(x, y, bob);
                    }

                    // obstacles
                    for (int i = 1; i <= 3; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var box = new BoundingBox(4, 0, 56, 36);
                        scenery.AddDecoration(x, y, 3);
                        scenery.AddObstacle(x, y, box);
                    }
                    break;

                case SceneryType.Rocks:
                    // decorations
                    for (int i = 1; i <= 6; i++)
                    {
                        var x = GlobalUtils.Rand(600) + 20;
                        var y = GlobalUtils.Rand(490) + 20;
                        scenery.AddDecoration(x, y, 9);
                    }
                    for (int i = 1; i <= 50; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var bob = GlobalUtils.Rand(1) + 1;
                        var hrev = GlobalUtils.Rand(1) == 0;
                        scenery.AddDecoration(x, y, bob, hrev);
                    }

                    // traps
                    for (int i = 1; i <= 5 / d; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var bob = GlobalUtils.Rand(1) + 6;
                        var box = new BoundingBox(20, 12, 70, 25);
                        scenery.AddDecoration(x, y, bob);
                        scenery.AddObstacle(x, y, box);
                    }
                    for (int i = 1; i <= 5 / d; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var box = new BoundingBox(16, 10, 34, 15);
                        scenery.AddDecoration(x, y, 5);
                        scenery.AddObstacle(x, y, box);
                    }

                    // obstacles
                    for (int i = 1; i <= 10 / d; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var bob = GlobalUtils.Rand(1) + 3;
                        var box = new BoundingBox(0, 0, 48, 36);
                        scenery.AddDecoration(x, y, bob);
                        scenery.AddObstacle(x, y, box);
                    }
                    for (int i = 1; i <= 5 / d; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var box = new BoundingBox(10, 0, 76, 62);
                        scenery.AddDecoration(x, y, 8);
                        scenery.AddObstacle(x, y, box);
                    }
                    break;

                case SceneryType.Desert:
                    // decorations
                    for (int i = 1; i <= 40; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var bob = GlobalUtils.Rand(3) + 1;
                        var hrev = GlobalUtils.Rand(1) == 0;
                        scenery.AddDecoration(x, y, bob);
                    }
                    for (int i = 1; i <= 10; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var hrev = GlobalUtils.Rand(1) == 0;
                        scenery.AddDecoration(x, y, 5);
                    }

                    // obstacles
                    for (int i = 1; i <= 10 / d; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var bob = GlobalUtils.Rand(1) + 6;
                        var box = new BoundingBox(0, 0, 48, 35);
                        scenery.AddDecoration(x, y, bob);
                        scenery.AddObstacle(x, y, box);
                    }
                    for (int i = 1; i <= 5 / d; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var box = new BoundingBox(0, 0, 64, 45);
                        scenery.AddDecoration(x, y, 8);
                        scenery.AddObstacle(x, y, box);
                    }
                    break;

                case SceneryType.Snow:
                    // decorations
                    for (int i = 1; i <= 30; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var hrev = GlobalUtils.Rand(1) == 0;
                        scenery.AddDecoration(x, y, 1);
                    }
                    for (int i = 1; i <= 15; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var hrev = GlobalUtils.Rand(1) == 0;
                        scenery.AddDecoration(x, y, 2);
                    }

                    // traps
                    for (int i = 1; i <= 5 / d; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var box = new BoundingBox(8, 0, 52, 40);
                        scenery.AddDecoration(x, y, 5);
                        scenery.AddObstacle(x, y, box);
                    }

                    // obstacles
                    for (int i = 1; i <= 10 / d; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var box = new BoundingBox(0, 0, 32, 40);
                        scenery.AddDecoration(x, y, 3);
                        scenery.AddObstacle(x, y, box);
                    }
                    for (int i = 1; i <= 1; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var box = new BoundingBox(0, 0, 64, 82);
                        scenery.AddDecoration(x, y, 6);
                        scenery.AddObstacle(x, y, box);
                    }
                    for (int i = 1; i <= 5 / d; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var box = new BoundingBox(0, 0, 64, 45);
                        scenery.AddDecoration(x, y, 8);
                        scenery.AddObstacle(x, y, box);
                    }

                    if (!isInsideCity)
                    {
                        // trees
                        for (int j = 0; j <= 3; j++)
                        {
                            var x2 = GlobalUtils.Rand(640);
                            var y2 = j * 100;
                            for (int i = 0; i <= 18; i++)
                            {
                                var x = x2 + GlobalUtils.Rand(80) - 40;
                                var y = y2 + (i * 4) - 60;
                                scenery.AddDecoration(x, y, 4);
                            }
                            var box1 = new BoundingBox(-30, -50, 100, 100);
                            scenery.AddObstacle(x2, y2, box1);
                        }
                    }
                    break;

                case SceneryType.Swamp:
                    // decorations
                    for (int i = 1; i <= 15; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var bob = GlobalUtils.Rand(1) + 1;
                        var hrev = GlobalUtils.Rand(1) == 0;
                        scenery.AddDecoration(x, y, bob, hrev);
                    }

                    // obstacles
                    for (int i = 1; i <= 2; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var box = new BoundingBox(25, 0, 29, 20);
                        scenery.AddDecoration(x, y, 3);
                        scenery.AddObstacle(x, y, box);
                    }
                    for (int i = 1; i <= 3; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var box = new BoundingBox(15, 0, 20, 30);
                        scenery.AddDecoration(x, y, 4);
                        scenery.AddObstacle(x, y, box);
                    }
                    for (int i = 1; i <= 6; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var hrev = GlobalUtils.Rand(1) == 0;
                        var box = new BoundingBox(4, 0, 40, 40);
                        scenery.AddDecoration(x, y, 5);
                        scenery.AddObstacle(x, y, box);
                    }

                    // traps
                    for (int i = 1; i <= 5; i++)
                    {
                        var (x, y) = GetDecorationPosition(scenery.Obstacles);
                        var box = new BoundingBox(0, 0, 64, 48);
                        scenery.AddObstacle(x, y, box);
                    }
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException(nameof(scenery), scenery.Type, "Unsuported scenery type");
            }
        }

        private static (int, int) GetDecorationPosition(IEnumerable<Obstacle> obstacles)
        {
            const int lx = 20, lszer = 600;
            const int ly = 20, lwys = 490;
            int x, y;
            do
            {
                x = GlobalUtils.Rand(lszer) + lx;
                y = GlobalUtils.Rand(lwys) + ly;
            }
            while (IsColliding(x, y, obstacles));
            return (x, y);
        }

        public void PositionCharacters(Army army, int zoneX, int zoneY, PlacementZone type, IEnumerable<TerrainObject> otherObjects)
        {
            var placedObjects = otherObjects.ToList();
            foreach (var character in army.Characters)
            {
                var (zoneXOffset, zoneYOffset) = DetermineZonePosition(zoneX, zoneY, type);

                int x, y;
                do
                {
                    x = GlobalUtils.Rand(200) + zoneXOffset + 16;
                    y = GlobalUtils.Rand(160) + zoneYOffset + 20;
                }
                while (IsColliding(x, y, placedObjects));

                character.X = x;
                character.Y = y;
                character.Box = new(-16, -20, 32, 20);
                placedObjects.Add(character);
            }
        }

        private static bool IsColliding(int x, int y, IEnumerable<TerrainObject> objects)
        {
            foreach (var obj in objects)
            {
                if (obj.BoxContains(x, y))
                {
                    return true;
                }
            }
            return false;
        }

        private static (int, int) DetermineZonePosition(int zoneX, int zoneY, PlacementZone type)
        {
            //TODO: replace this magic numbers (200/160) by screen width/height or something 
            var x1 = zoneX * 200;
            var y1 = zoneY * 160;

            if (type == PlacementZone.OtherThan)
            {
                var (newZoneX, newZoneY) = FindDifferentZone(zoneX, zoneY);
                return DetermineZonePosition(newZoneX, newZoneY, PlacementZone.Fixed);
            }
            else if (type == PlacementZone.RandomX)
            {
                x1 = GlobalUtils.Rand(2) * 200;
            }
            else if (type == PlacementZone.RandomY)
            {
                y1 = GlobalUtils.Rand(2) * 160;
            }

            return (x1, y1);
        }

        private static (int, int) FindDifferentZone(int zoneX, int zoneY)
        {
            int newX = zoneX;
            int newY = zoneY;
            while (newX == zoneX && newY == zoneY)
            {
                newX = GlobalUtils.Rand(2);
                newY = GlobalUtils.Rand(2);
            }
            return (newX, newY);
        }
    }
}