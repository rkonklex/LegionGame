using AwaitableCoroutine;
using Legion.Model.Types;

namespace Legion.Model
{
    public interface IArmyActivities
    {
        Coroutine Encounter(Army army, EncounterType stuckInSwamp);
        Coroutine Hunt(Army army, TerrainType terrainType);
    }
}