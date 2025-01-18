using System.Diagnostics;
using AwaitableCoroutine;
using Legion.Model.Types;

namespace Legion.Model
{
    public interface IViewSwitcher
    {
        // switching between Views
        void OpenMenu();
        void OpenMap();
        void OpenTerrain(TerrainActionContext context);
    }

    public static class ViewSwitcherExtensions
    {
        public static Coroutine OpenTerrainAsync(this IViewSwitcher viewSwitcher, Army userArmy, Army enemyArmy)
        {
            Debug.Assert(userArmy.IsUserControlled);
            Debug.Assert(!enemyArmy.IsUserControlled);

            var context = new TerrainActionContext
            {
                UserArmy = userArmy,
                EnemyArmy = enemyArmy,
                Type = TerrainActionType.Battle,
            };
            viewSwitcher.OpenTerrain(context);
            return context.ActionFinished;
        }
    }
}