using AwaitableCoroutine;
using Legion.Model;
using Legion.Model.Types;
using System;

namespace Legion.Controllers.Terrain
{
    public class TerrainController : ITerrainController
    {
        private readonly ILegionConfig _legionConfig;
        private readonly Func<TerrainActionContext, ITerrainTurnProcessor> _turnProcessorFactory;

        public TerrainController(ILegionConfig legionConfig, Func<TerrainActionContext, ITerrainTurnProcessor> turnProcessorFactory)
        {
            _legionConfig = legionConfig;
            _turnProcessorFactory = turnProcessorFactory;
        }

        public bool IsPaused { get; set; }
        public TerrainActionContext Context { get; set; }
        public Scenery Scenery => Context?.Scenery;
        public Army UserArmy => Context?.UserArmy;
        public Army EnemyArmy => Context?.EnemyArmy;

        public void SetupTerrainAction(TerrainActionContext context)
        {
            IsPaused = false;
            Context = context;
        }

        public async Coroutine StartTerrainAction()
        {
            var turnProcessor = _turnProcessorFactory(Context);

            while (Context is not null && !UserArmy.IsKilled)
            {
                if (IsPaused)
                {
                    await Coroutine.Yield();
                }
                else
                {
                    await turnProcessor.ProcessTurn();
                    await Coroutine.DelayCount(4);
                }
            }
        }

        public void EndTerrainAction()
        {
            foreach (var character in UserArmy.Characters)
            {
                character.CurrentAction = CharacterActionType.None;
            }
            foreach (var character in EnemyArmy.Characters)
            {
                character.CurrentAction = CharacterActionType.None;
            }

            Context = null;
        }
    }
}