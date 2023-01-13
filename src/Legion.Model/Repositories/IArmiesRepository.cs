using System.Collections.Generic;
using Legion.Model.Types;
using Legion.Model.Types.Definitions;

namespace Legion.Model.Repositories
{
    public interface IArmiesRepository
    {
        List<Army> Armies { get; }
        Army CreateUserArmy(Player owner, int charactersCount);
        Army CreateNpcArmy(Player owner, int charactersCount, int power, CharacterDefinition charactersType = null);
        Army CreateTempArmy(int charactersCount, int power, CharacterDefinition charactersType = null);
        Army CreateTempArmyForHunt(TerrainType terrainType);
        void KillArmy(Army army);
    }
}