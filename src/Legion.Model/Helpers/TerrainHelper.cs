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