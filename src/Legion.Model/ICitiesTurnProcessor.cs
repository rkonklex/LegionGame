using AwaitableCoroutine;

namespace Legion.Model
{
    public interface ICitiesTurnProcessor
    {
        bool IsProcessingTurn { get; }
        Coroutine NextTurn();
    }
}