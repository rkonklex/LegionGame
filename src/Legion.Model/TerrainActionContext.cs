using System;
using AwaitableCoroutine;
using Legion.Model.Types;
using Legion.Utils;

namespace Legion.Model
{
    public class TerrainActionContext
    {
        public Army UserArmy { get; set; }
        public Army EnemyArmy { get; set; }
        public TerrainActionType Type { get; set; }

        private readonly CoroutineCompletionSource _completionSource = new();
        public Coroutine ActionFinished => _completionSource.Coroutine;
        public void NotifyActionFinished() => _completionSource.SetCompleted();
    }
}