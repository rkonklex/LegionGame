using Legion.Model.Types;
using Legion.Model.Types.Definitions;

namespace Legion.Model.Repositories
{
    public interface ICharactersRepository
    {
        Character CreateWarrior(RaceDefinition type);
        Character CreateNpc(CharacterDefinition type, int power);
    }
}
