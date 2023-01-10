using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Encounter(Army army, EncounterType encounterType)
        {
            Army encounterArmy = null;
            var encounterMessage = new Message();
            encounterMessage.MapObjects = new List<MapObject> { army };

            var characterTypes = _characterDefinitionsRepository;
            switch (encounterType)
            {
                case EncounterType.RabidWolves:
                    encounterArmy = CreateTempArmy(GlobalUtils.Rand(5) + 5, characterTypes.Wolf);
                    encounterMessage.Type = MessageType.ArmyEncounteredRabidWolves;
                    break;

                case EncounterType.Bandits:
                    encounterArmy = CreateTempArmy(GlobalUtils.Rand(5) + 5);
                    encounterMessage.Type = MessageType.ArmyEncounteredBandits;
                    break;

                case EncounterType.StuckInSwamp:
                    encounterArmy = CreateTempArmy(GlobalUtils.Rand(5), characterTypes.Gloom);
                    encounterMessage.Type = MessageType.ArmyStuckInSwamp;
                    break;

                case EncounterType.ForestTrolls:
                    encounterArmy = CreateTempArmy(GlobalUtils.Rand(5) + 5, characterTypes.Troll);
                    encounterMessage.Type = MessageType.ArmyEncounteredForestTrolls;
                    break;

                case EncounterType.Gargoyl:
                    encounterArmy = CreateTempArmy(GlobalUtils.Rand(1) + 1, characterTypes.Gargoyle);
                    encounterMessage.Type = MessageType.ArmyEncounteredGargoyl;
                    break;

                case EncounterType.LoneKnight:
                    encounterArmy = CreateTempArmy(1);
                    encounterArmy.Characters.First().Experience = GlobalUtils.Rand(30) + 20;
                    encounterArmy.Characters.First().Aggression = 40;
                    encounterMessage.Type = MessageType.ArmyEncounteredLoneKnight;
                    break;

                case EncounterType.CaveEntrance:
                    encounterArmy = CreateTempArmy(5, characterTypes.Spider);
                    for (int i = 0; i < 5; i++)
                    {
                        var character = _armiesRepository.AddNpc(encounterArmy, _legionInfo.Power);
                        character.Aggression = 150 + GlobalUtils.Rand(50);
                    }
                    encounterMessage.Type = MessageType.ArmyEncounteredCaveEntrance;
                    break;

                default: throw new ArgumentException("Invalid EncounterType", nameof(encounterType));
            }

            var battleContext = new TerrainActionContext();
            battleContext.UserArmy = army;
            battleContext.EnemyArmy = encounterArmy;
            battleContext.Type = TerrainActionType.Battle;

            encounterMessage.OnClose = () => { _viewSwitcher.OpenTerrain(battleContext); };
            _messagesService.ShowMessage(encounterMessage);
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