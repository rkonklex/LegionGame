using System;
using System.Collections.Generic;
using System.Linq;
using Legion.Model.Types;
using Legion.Model.Types.Definitions;
using Legion.Utils;

namespace Legion.Model.Repositories
{
    public class ArmiesRepository : IArmiesRepository
    {
        private readonly IRaceDefinitionsRepository _raceDefinitionsRepository;
        private readonly ICreatureDefinitionsRepository _creatureDefinitionsRepository;
        private readonly ICharactersRepository _charactersRepository;

        public ArmiesRepository(IRaceDefinitionsRepository raceDefinitionsRepository,
            ICreatureDefinitionsRepository creatureDefinitionsRepository,
            ICharactersRepository charactersRepository)
        {
            _raceDefinitionsRepository = raceDefinitionsRepository;
            _creatureDefinitionsRepository = creatureDefinitionsRepository;
            _charactersRepository = charactersRepository;

            Armies = new List<Army>();
        }

        public List<Army> Armies { get; private set; }

        public Army CreateArmy(Player owner, int charactersCount, CharacterDefinition charactersType = null)
        {
            var army = new Army();
            army.Owner = owner;

            if (army.Owner != null)
            {
                var armyId = Armies.Count(a => a.Owner == army.Owner) + 1;

                if (army.Owner.IsUserControlled)
                {
                    army.Name = "Legion " + armyId;
                }
                else if (army.Owner.IsChaosControlled)
                {
                    army.Name = "Wojownicy Chaosu";
                    army.DaysToGetInfo = 30;
                }
                else
                {
                    var postfix = army.Owner.Name.EndsWith("I", StringComparison.OrdinalIgnoreCase) ? "ego" : "a";
                    army.Name = armyId + " Legion " + army.Owner.Name + postfix;
                    army.DaysToGetInfo = 30;
                    //army.Aggression = 150 + Rand.Next(50) + dataManager.Power;
                }
            }

            army.Food = GlobalUtils.Rand(200);

            for (var i = 0; i < charactersCount; i++)
            {
                var type = charactersType ?? _raceDefinitionsRepository.GetRandomWarrior();
                var character = _charactersRepository.CreateCharacter(type);
                army.Characters.Add(character);
            }

            Armies.Add(army);

            return army;
        }

        public Army CreateTempArmy(int charactersCount, CharacterDefinition charactersType = null)
        {
            var army = new Army();

            for (var i = 0; i < charactersCount; i++)
            {
                var type = charactersType ?? _raceDefinitionsRepository.GetRandomWarrior();
                var character = _charactersRepository.CreateCharacter(type);
                army.Characters.Add(character);
            }

            return army;
        }

        public Army CreateTempArmyForHunt(TerrainType terrainType)
        {
            int creatureCountRoll = 9;
            CreatureDefinition creatureType;

            if (terrainType == TerrainType.Swamp)
            {
                creatureType = _creatureDefinitionsRepository.Gloom;
            }
            else
            {
                switch (GlobalUtils.Rand(13))
                {
                    case < 5:
                        creatureType = _creatureDefinitionsRepository.Hog;
                        break;
                    case > 4 and < 8:
                        creatureType = _creatureDefinitionsRepository.Wolf;
                        break;
                    case 8 or 9:
                        creatureType = _creatureDefinitionsRepository.Gargoyle;
                        creatureCountRoll = 3;
                        break;
                    case 10:
                        creatureType = _creatureDefinitionsRepository.Skeerial;
                        creatureCountRoll = 5;
                        break;
                    case > 10:
                        creatureType = _creatureDefinitionsRepository.Varpoon;
                        break;
                }
            }

            var creatureCount = GlobalUtils.Rand(creatureCountRoll) + 1;
            var creatureArmy = CreateTempArmy(creatureCount, creatureType);
            return creatureArmy;
        }

        public void KillArmy(Army army)
        {
            foreach (var a in Armies)
            {
                if (a.Target == army)
                {
                    a.Target = null;
                }
            }

            army.Characters.Clear();
            army.Target = null;

            if (Armies.Contains(army))
            {
                Armies.Remove(army);
            }
        }
    }
}