using AwaitableCoroutine;
using Legion.Model.Types;

namespace Legion.Model
{
    public interface IBattleManager
    {
        Coroutine AttackOnArmy(Army army, Army targetArmy, WorldDirection movementDirection);
        Coroutine AttackOnCity(Army army, City city);
    }
}