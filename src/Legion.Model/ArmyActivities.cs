using System;
using System.Collections.Generic;
using System.Linq;
using AwaitableCoroutine;
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

        public async Coroutine Encounter(Army army, EncounterType encounterType)
        {
            Army encounterArmy = null;
            MessageType? encounterMessage = null;

            var characterTypes = _characterDefinitionsRepository;
            switch (encounterType)
            {
                case EncounterType.RabidWolves:
                    encounterArmy = CreateTempArmy(GlobalUtils.Rand(5) + 5, characterTypes.Wolf);
                    encounterMessage = MessageType.ArmyEncounteredRabidWolves;
                    break;

                case EncounterType.Bandits:
                    encounterArmy = CreateTempArmy(GlobalUtils.Rand(5) + 5);
                    encounterMessage = MessageType.ArmyEncounteredBandits;
                    break;

                case EncounterType.StuckInSwamp:
                    encounterArmy = CreateTempArmy(GlobalUtils.Rand(5), characterTypes.Gloom);
                    encounterMessage = MessageType.ArmyStuckInSwamp;
                    break;

                case EncounterType.ForestTrolls:
                    encounterArmy = CreateTempArmy(GlobalUtils.Rand(5) + 5, characterTypes.Troll);
                    encounterMessage = MessageType.ArmyEncounteredForestTrolls;
                    break;

                case EncounterType.Gargoyl:
                    encounterArmy = CreateTempArmy(GlobalUtils.Rand(1) + 1, characterTypes.Gargoyle);
                    encounterMessage = MessageType.ArmyEncounteredGargoyl;
                    break;

                case EncounterType.LoneKnight:
                    encounterArmy = CreateTempArmy(1);
                    encounterArmy.Characters.First().Experience = GlobalUtils.Rand(30) + 20;
                    encounterArmy.Characters.First().Aggression = 40;
                    encounterMessage = MessageType.ArmyEncounteredLoneKnight;
                    break;

                case EncounterType.CaveEntrance:
                    encounterArmy = CreateTempArmy(5, characterTypes.Spider);
                    for (int i = 0; i < 5; i++)
                    {
                        var character = _armiesRepository.AddNpc(encounterArmy, _legionInfo.Power);
                        character.Aggression = 150 + GlobalUtils.Rand(50);
                    }
                    encounterMessage = MessageType.ArmyEncounteredCaveEntrance;
                    break;

                default: throw new ArgumentException("Invalid EncounterType", nameof(encounterType));
            }

            await _messagesService.ShowMessageAsync(encounterMessage.Value, army);

            await _viewSwitcher.OpenTerrainAsync(army, encounterArmy);
        }

        public async Coroutine Hunt(Army army, TerrainType terrainType)
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

            await _messagesService.ShowMessageAsync(MessageType.ArmyTrackedDownBeast, army);

            await _viewSwitcher.OpenTerrainAsync(army, creatureArmy);

            army.CurrentAction = ArmyActions.Camping;
        }

        private Army CreateTempArmy(int charactersCount, CharacterDefinition charactersType = null)
        {
            return _armiesRepository.CreateTempArmy(charactersCount, _legionInfo.Power, charactersType);
        }
    }
}