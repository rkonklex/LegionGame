using System.Collections.Generic;
using Legion.Model.Types.Definitions;

namespace Legion.Model.Repositories
{
    public interface ICreatureDefinitionsRepository
    {
        List<CreatureDefinition> AllCreatures { get; }

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
    }
}