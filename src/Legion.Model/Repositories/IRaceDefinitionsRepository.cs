using Legion.Model.Types.Definitions;
using System.Collections.Generic;

namespace Legion.Model.Repositories
{
    public interface IRaceDefinitionsRepository
    {
        List<RaceDefinition> AllRaces { get; }

        RaceDefinition Amazonian { get; }
        RaceDefinition Barbarian { get; }
        RaceDefinition Dwarf { get; }
        RaceDefinition Elf { get; }
        RaceDefinition Knight { get; }
        RaceDefinition Ogre { get; }
        RaceDefinition Orc { get; }
        RaceDefinition Sorcerer { get; }
        RaceDefinition Troll { get; }
        RaceDefinition Villager { get; }
        List<RaceDefinition> AllWarriorRaces { get; }

        RaceDefinition GetRandomWarrior();
    }
}