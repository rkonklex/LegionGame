using AwaitableCoroutine;
using Legion.Model;
using Legion.Model.Types;

namespace Legion.Controllers.Terrain
{
    public interface ITerrainController
    {
        bool IsPaused { get; set; }
        TerrainActionContext Context { get; set; }
        Army EnemyArmy { get; }
        Army UserArmy { get; }

        void SetupTerrainAction(TerrainActionContext context);
        Coroutine StartTerrainAction();
        void EndTerrainAction();
    }
}