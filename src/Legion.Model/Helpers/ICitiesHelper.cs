using Legion.Model.Types;

namespace Legion.Model.Helpers
{
    public interface ICitiesHelper
    {
        void UpdatePriceModificators(City city);
        TerrainType GetCityTerrainType(City city);
    }
}