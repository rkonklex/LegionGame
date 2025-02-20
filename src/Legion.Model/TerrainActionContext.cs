using System;
using AwaitableCoroutine;
using Legion.Model.Types;
using Legion.Utils;

namespace Legion.Model
{
    public class TerrainActionContext
    {
        public Scenery Scenery { get; set; }
        public Army UserArmy { get; set; }
        public Army EnemyArmy { get; set; }
        public TerrainActionType Type { get; set; }

        private readonly CoroutineCompletionSource _completionSource = new();
        public Coroutine ActionFinished => _completionSource.Coroutine;
        public void NotifyActionFinished() => _completionSource.SetCompleted();

        public bool HitTest(int x, int y, out TerrainObject hitObject)
        {
            if (EnemyArmy.HitTest(x, y, out var enemyCharacter))
            {
                hitObject = enemyCharacter;
                return true;
            }
            if (UserArmy.HitTest(x, y, out var userCharacter))
            {
                hitObject = userCharacter;
                return true;
            }

            return Scenery.HitTest(x, y, out hitObject);
        }
    }
}