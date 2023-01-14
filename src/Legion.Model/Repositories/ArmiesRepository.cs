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
        private readonly ICharacterDefinitionsRepository _characterDefinitionsRepository;
        private readonly ICharactersRepository _charactersRepository;

        public ArmiesRepository(ICharacterDefinitionsRepository characterDefinitionsRepository,
            ICharactersRepository charactersRepository)
        {
            _characterDefinitionsRepository = characterDefinitionsRepository;
            _charactersRepository = charactersRepository;

            Armies = new List<Army>();
        }

        public List<Army> Armies { get; private set; }

        public Army CreateUserArmy(Player owner, int charactersCount)
        {
            var army = new Army();
            var armyId = FindNewArmyId(army.Owner);
            army.Owner = owner;
            army.Name = "Legion " + armyId;
            army.Food = GlobalUtils.Rand(200);

            for (var i = 0; i < charactersCount; i++)
            {
                AddWarrior(army);
            }

            Armies.Add(army);

            return army;
        }

        public Army CreateNpcArmy(Player owner, int charactersCount, int power, CharacterDefinition charactersType = null)
        {
            var army = new Army();
            var armyId = FindNewArmyId(army.Owner);
            army.Owner = owner;
            army.DaysToGetInfo = 30;

            if (army.IsChaosControlled)
            {
                army.Name = "Wojownicy Chaosu";
            }
            else
            {
                var postfix = army.Owner.Name.EndsWith("I", StringComparison.OrdinalIgnoreCase) ? "ego" : "a";
                army.Name = armyId + " Legion " + army.Owner.Name + postfix;
                //army.Aggression = 150 + Rand.Next(50) + dataManager.Power;
            }

            for (var i = 0; i < charactersCount; i++)
            {
                AddNpc(army, power, charactersType);
            }

            Armies.Add(army);

            return army;
        }

        private int FindNewArmyId(Player owner)
        {
            return Armies.Count(a => a.Owner == owner) + 1;
        }

        public Army CreateTempArmy(int charactersCount, int power, CharacterDefinition charactersType = null)
        {
            var army = new Army();

            for (var i = 0; i < charactersCount; i++)
            {
                AddNpc(army, power, charactersType);
            }

            return army;
        }

        public Army CreateTempArmyForHunt(TerrainType terrainType)
        {
            int creatureCountRoll = 9;
            CreatureDefinition creatureType;

            if (terrainType == TerrainType.Swamp)
            {
                creatureType = _characterDefinitionsRepository.Gloom;
            }
            else
            {
                switch (GlobalUtils.Rand(13))
                {
                    case < 5:
                        creatureType = _characterDefinitionsRepository.Hog;
                        break;
                    case > 4 and < 8:
                        creatureType = _characterDefinitionsRepository.Wolf;
                        break;
                    case 8 or 9:
                        creatureType = _characterDefinitionsRepository.Gargoyle;
                        creatureCountRoll = 3;
                        break;
                    case 10:
                        creatureType = _characterDefinitionsRepository.Skeerial;
                        creatureCountRoll = 5;
                        break;
                    case > 10:
                        creatureType = _characterDefinitionsRepository.Varpoon;
                        break;
                }
            }

            var creatureCount = GlobalUtils.Rand(creatureCountRoll) + 1;
            var creatureArmy = CreateTempArmy(creatureCount, 0, creatureType);
            return creatureArmy;
        }

        public Character AddWarrior(Army army, RaceDefinition charactersType = null)
        {
            var type = charactersType ?? _characterDefinitionsRepository.GetRandomWarrior();
            var character = _charactersRepository.CreateWarrior(type);
            army.Characters.Add(character);
            return character;
        }

        public Character AddNpc(Army army, int power, CharacterDefinition charactersType = null)
        {
            var type = charactersType ?? _characterDefinitionsRepository.GetRandomWarrior();
            var character = _charactersRepository.CreateNpc(type, power);
            army.Characters.Add(character);
            return character;
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