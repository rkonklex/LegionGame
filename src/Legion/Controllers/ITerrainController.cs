using Legion.Model.Types;

namespace Legion.Controllers
{
    public interface ITerrainController
    {
        bool IsPaused { get; set; }
        Army EnemyArmy { get; set; }
        Army UserArmy { get; set; }
    }
}