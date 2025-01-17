using System.Collections.Generic;
using AwaitableCoroutine;
using Legion.Model.Types;

namespace Legion.Controllers.Map
{
    public interface IMapController
    {
        List<City> Cities { get; }
        List<Army> Armies { get; }
        bool IsProcessingTurn { get; }
        Coroutine StartNextTurn();
    }
}