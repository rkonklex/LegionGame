using System.Collections.Generic;
using Legion.Model.Repositories;
using Legion.Model.Types;

namespace Legion.Model
{
    public class ArmyActivities : IArmyActivities
    {
        private readonly IArmiesRepository _armiesRepository;
        private readonly IMessagesService _messagesService;
        private readonly IViewSwitcher _viewSwitcher;

        public ArmyActivities(IArmiesRepository armiesRepository,
            IMessagesService messagesService,
            IViewSwitcher viewSwitcher)
        {
            _armiesRepository = armiesRepository;
            _messagesService = messagesService;
            _viewSwitcher = viewSwitcher;
        }

        public void Hunt(Army army, TerrainType terrainType)
        {
            var creatureArmy = _armiesRepository.CreateTempArmyForHunt(terrainType);

            var battleContext = new TerrainActionContext();
            battleContext.UserArmy = army;
            battleContext.EnemyArmy = creatureArmy;
            battleContext.Type = TerrainActionType.Battle;
            battleContext.ActionAfter = () =>
            {
                army.CurrentAction = ArmyActions.Camping;
            };

            var huntMessage = new Message();
            huntMessage.Type = MessageType.ArmyTrackedDownBeast;
            huntMessage.MapObjects = new List<MapObject> { army };
            huntMessage.OnClose = () => { _viewSwitcher.OpenTerrain(battleContext); };

            _messagesService.ShowMessage(huntMessage);
        }
    }
}