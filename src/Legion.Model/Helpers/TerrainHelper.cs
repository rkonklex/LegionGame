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
            var d = city is null ? 1 : 2;
            var scenery = new Scenery(sceneryType);

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
                var (x, y) = GetDecorationPosition();
                var bob = GlobalUtils.Rand(2) + 1;
                var hrev = GlobalUtils.Rand(1) == 0;
                var box = new BoundingBox(4, 4, 24, 18);
                scenery.AddDecoration(x, y, bob, hrev);
                scenery.AddObstacle(x, y, box);
            }

            if (city is null)
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

            return scenery;
        }

        private static (int, int) GetDecorationPosition()
        {
            const int lx = 20, lszer = 600;
            const int ly = 20, lwys = 490;
            return (GlobalUtils.Rand(lszer) + lx, GlobalUtils.Rand(lwys) + ly);
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