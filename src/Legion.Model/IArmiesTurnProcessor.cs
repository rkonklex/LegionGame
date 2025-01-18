using AwaitableCoroutine;
using Legion.Model.Types;

namespace Legion.Model
{
    public interface IArmiesTurnProcessor
    {
        bool IsProcessingTurn { get; }

        Coroutine NextTurn();
    }
}