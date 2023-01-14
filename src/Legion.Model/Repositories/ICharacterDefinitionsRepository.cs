using System.Collections.Generic;
using Legion.Model.Types.Definitions;

namespace Legion.Model.Repositories
{
    public interface ICharacterDefinitionsRepository
    {
        List<CreatureDefinition> AllCreatures { get; }
        List<RaceDefinition> AllRaces { get; }
        List<RaceDefinition> AllWarriorRaces { get; }

        CreatureDefinition Gargoyle { get; }
        CreatureDefinition Skeleton { get; }
        CreatureDefinition Wolf { get; }
        CreatureDefinition Hog { get; }
        CreatureDefinition Gloom { get; }
        CreatureDefinition Varpoon { get; }
        CreatureDefinition Skeerial { get; }
        CreatureDefinition Humanoid { get; }
        CreatureDefinition Spider { get; }
        CreatureDefinition Boss { get; }

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

        RaceDefinition GetRandomWarrior();
    }
}