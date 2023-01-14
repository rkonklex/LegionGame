using System.Collections.Generic;
using Legion.Model.Repositories;
using Legion.Model.Types;
using Legion.Model.Types.Definitions;
using Legion.Utils;

namespace Legion.Model
{
    public class ArmyActivities : IArmyActivities
    {
        private readonly IArmiesRepository _armiesRepository;
        private readonly ICharacterDefinitionsRepository _characterDefinitionsRepository;
        private readonly ILegionInfo _legionInfo;
        private readonly IMessagesService _messagesService;
        private readonly IViewSwitcher _viewSwitcher;

        public ArmyActivities(IArmiesRepository armiesRepository,
            ICharacterDefinitionsRepository characterDefinitionsRepository,
            ILegionInfo legionInfo,
            IMessagesService messagesService,
            IViewSwitcher viewSwitcher)
        {
            _armiesRepository = armiesRepository;
            _characterDefinitionsRepository = characterDefinitionsRepository;
            _legionInfo = legionInfo;
            _messagesService = messagesService;
            _viewSwitcher = viewSwitcher;
        }

        public void Hunt(Army army, TerrainType terrainType)
        {
            Army creatureArmy = null;
            var creatureTypes = _characterDefinitionsRepository;

            if (terrainType == TerrainType.Swamp)
            {
                creatureArmy = CreateTempArmy(GlobalUtils.Rand(9) + 1, creatureTypes.Gloom);
            }
            else
            {
                switch (GlobalUtils.Rand(13))
                {
                    case < 5:
                        creatureArmy = CreateTempArmy(GlobalUtils.Rand(9) + 1, creatureTypes.Hog);
                        break;
                    case > 4 and < 8:
                        creatureArmy = CreateTempArmy(GlobalUtils.Rand(9) + 1, creatureTypes.Wolf);
                        break;
                    case 8 or 9:
                        creatureArmy = CreateTempArmy(GlobalUtils.Rand(3) + 1, creatureTypes.Gargoyle);
                        break;
                    case 10:
                        creatureArmy = CreateTempArmy(GlobalUtils.Rand(5) + 1, creatureTypes.Skeerial);
                        break;
                    case > 10:
                        creatureArmy = CreateTempArmy(GlobalUtils.Rand(9) + 1, creatureTypes.Varpoon);
                        break;
                }
            }

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

        private Army CreateTempArmy(int charactersCount, CharacterDefinition charactersType = null)
        {
            return _armiesRepository.CreateTempArmy(charactersCount, _legionInfo.Power, charactersType);
        }
    }
}