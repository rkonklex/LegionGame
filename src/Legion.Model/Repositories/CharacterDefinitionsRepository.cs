using System.Collections.Generic;
using System.Linq;
using Legion.Model.Types.Definitions;
using Legion.Utils;

namespace Legion.Model.Repositories
{
    public class CharacterDefinitionsRepository : ICharacterDefinitionsRepository
    {
        private readonly IDefinitionsRepository _definitionsRepository;

        public CharacterDefinitionsRepository(IDefinitionsRepository definitionsRepository)
        {
            _definitionsRepository = definitionsRepository;
        }

        public List<CreatureDefinition> AllCreatures => _definitionsRepository.Creatures;
        public List<RaceDefinition> AllRaces => _definitionsRepository.Races;
        public List<RaceDefinition> AllWarriorRaces => AllRaces.Where(c => !c.Name.Equals("villager")).ToList();

        public CreatureDefinition Skeleton => FindCreatureByName("skeletor");

        public CreatureDefinition Gargoyle => FindCreatureByName("gargoyl");

        public CreatureDefinition Wolf => FindCreatureByName("wolf");

        public CreatureDefinition Hog => FindCreatureByName("hog");

        public CreatureDefinition Gloom => FindCreatureByName("gloom");

        public CreatureDefinition Varpoon => FindCreatureByName("varpoon");

        public CreatureDefinition Skeerial => FindCreatureByName("skeerial");

        public CreatureDefinition Humanoid => FindCreatureByName("humanoid");

        public CreatureDefinition Spider => FindCreatureByName("spider");

        public CreatureDefinition Boss => FindCreatureByName("boss");

        private CreatureDefinition FindCreatureByName(string name)
        {
            return AllCreatures.First(c => c.Name.Equals(name));
        }

        public RaceDefinition Barbarian => FindRaceByName("barbarian");

        public RaceDefinition Orc => FindRaceByName("orc");

        public RaceDefinition Elf => FindRaceByName("elf");

        public RaceDefinition Dwarf => FindRaceByName("dwarf");

        public RaceDefinition Amazonian => FindRaceByName("amazon");

        public RaceDefinition Ogre => FindRaceByName("ogre");

        public RaceDefinition Troll => FindRaceByName("troll");

        public RaceDefinition Knight => FindRaceByName("knight");

        public RaceDefinition Sorcerer => FindRaceByName("sorcerer");

        public RaceDefinition Villager => FindRaceByName("villager");

        public RaceDefinition GetRandomWarrior()
        {
            var warriors = AllWarriorRaces;
            return warriors[GlobalUtils.Rand(warriors.Count - 1)];
        }

        private RaceDefinition FindRaceByName(string name)
        {
            return AllRaces.First(c => c.Name.Equals(name));
        }
    }
}
