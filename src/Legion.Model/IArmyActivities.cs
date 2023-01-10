using Legion.Model.Types;

namespace Legion.Model
{
    public interface IArmyActivities
    {
        void Encounter(Army army, EncounterType stuckInSwamp);
        void Hunt(Army army, TerrainType terrainType);
    }
}