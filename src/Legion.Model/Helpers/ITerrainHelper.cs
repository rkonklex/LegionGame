using Legion.Model.Helpers;
using Legion.Model.Types;
using System.Collections.Generic;

namespace Legion.Model
{
    public interface ITerrainHelper
    {
        TerrainActionContextBuilder BuildTerrainActionContext();
        Scenery CreateScenery(SceneryType sceneryType, City city = null);
        void PositionCharacters(Army army, int zoneX, int zoneY, PlacementZone type, IEnumerable<TerrainObject> otherObjects);
    }

    public enum PlacementZone
    {
        Fixed,
        OtherThan,
        RandomX,
        RandomY,
    }
}