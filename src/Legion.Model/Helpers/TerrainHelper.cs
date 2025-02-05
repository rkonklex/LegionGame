using Legion.Model.Helpers;
using Legion.Model.Types;
using Legion.Utils;

namespace Legion.Model
{
    public class TerrainHelper : ITerrainHelper
    {
        public TerrainActionContextBuilder BuildTerrainActionContext()
        {
            return new TerrainActionContextBuilder(this);
        }

        public void PositionCharacters(Army army, int zoneX, int zoneY, PlacementZone type)
        {
            foreach (var character in army.Characters)
            {
                var (x1, y1) = DetermineZonePosition(zoneX, zoneY, type);
                do
                {
                    character.X = GlobalUtils.Rand(200) + x1 + 16;
                    character.Y = GlobalUtils.Rand(160) + y1 + 20;
                } while (false); //TODO: while there is no other things in that position
            }
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