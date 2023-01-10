using System.Collections.Generic;
using System.Linq;
using Legion.Model.Types.Definitions;
using Legion.Utils;

namespace Legion.Model.Repositories
{
    public class RaceDefinitionsRepository : IRaceDefinitionsRepository
    {
        private readonly IDefinitionsRepository _definitionsRepository;

        public RaceDefinitionsRepository(IDefinitionsRepository definitionsRepository)
        {
            _definitionsRepository = definitionsRepository;
        }

        public List<RaceDefinition> AllRaces => _definitionsRepository.Races;
        public List<RaceDefinition> AllWarriorRaces => AllRaces.Where(c => !c.Name.Equals("villager")).ToList();

        public RaceDefinition Barbarian => FindByName("barbarian");

        public RaceDefinition Orc => FindByName("orc");

        public RaceDefinition Elf => FindByName("elf");

        public RaceDefinition Dwarf => FindByName("dwarf");

        public RaceDefinition Amazonian => FindByName("amazon");

        public RaceDefinition Ogre => FindByName("ogre");

        public RaceDefinition Troll => FindByName("troll");

        public RaceDefinition Knight => FindByName("knight");

        public RaceDefinition Sorcerer => FindByName("sorcerer");

        public RaceDefinition Villager => FindByName("villager");

        public RaceDefinition GetRandomWarrior()
        {
            var warriors = AllWarriorRaces;
            return warriors[GlobalUtils.Rand(warriors.Count - 1)];
        }

        private RaceDefinition FindByName(string name)
        {
            return AllRaces.First(c => c.Name.Equals(name));
        }
    }
}
