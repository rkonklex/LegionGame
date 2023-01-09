using System.Collections.Generic;
using System.Linq;
using Legion.Model.Types.Definitions;

namespace Legion.Model.Repositories
{
    public class CreatureDefinitionsRepository : ICreatureDefinitionsRepository
    {
        private readonly IDefinitionsRepository _definitionsRepository;

        public CreatureDefinitionsRepository(IDefinitionsRepository definitionsRepository)
        {
            _definitionsRepository = definitionsRepository;
        }

        public List<CreatureDefinition> AllCreatures => _definitionsRepository.Creatures;

        public CreatureDefinition Skeleton => FindByName("skeletor");

        public CreatureDefinition Gargoyle => FindByName("gargoyl");

        public CreatureDefinition Wolf => FindByName("wolf");

        public CreatureDefinition Hog => FindByName("hog");

        public CreatureDefinition Gloom => FindByName("gloom");

        public CreatureDefinition Varpoon => FindByName("varpoon");

        public CreatureDefinition Skeerial => FindByName("skeerial");

        public CreatureDefinition Humanoid => FindByName("humanoid");

        public CreatureDefinition Spider => FindByName("spider");

        public CreatureDefinition Boss => FindByName("boss");

        private CreatureDefinition FindByName(string name)
        {
            return AllCreatures.First(c => c.Name.Equals(name));
        }
    }
}
