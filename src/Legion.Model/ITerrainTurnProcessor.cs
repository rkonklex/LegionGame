using AwaitableCoroutine;

namespace Legion.Model
{
    public interface ITerrainTurnProcessor
    {
        Coroutine ProcessTurn();
    }
}