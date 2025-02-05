using Legion.Model.Helpers;
using Legion.Model.Types;

namespace Legion.Model
{
    public interface ITerrainHelper
    {
        TerrainActionContextBuilder BuildTerrainActionContext();
        void PositionCharacters(Army army, int zoneX, int zoneY, PlacementZone type);
    }

    public enum PlacementZone
    {
        Fixed,
        OtherThan,
        RandomX,
        RandomY,
    }
}