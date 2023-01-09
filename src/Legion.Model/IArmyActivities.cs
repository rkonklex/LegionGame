using Legion.Model.Types;

namespace Legion.Model
{
    public interface IArmyActivities
    {
        void Hunt(Army army, TerrainType terrainType);
    }
}