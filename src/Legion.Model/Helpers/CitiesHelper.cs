using System;
using Legion.Model.Repositories;
using Legion.Model.Types;
using Legion.Model.Types.Definitions;
using Legion.Utils;

namespace Legion.Model.Helpers
{
    public class CitiesHelper : ICitiesHelper
    {
        private readonly IDefinitionsRepository _definitionsRepository;

        public CitiesHelper(IDefinitionsRepository definitionsRepository)
        {
            _definitionsRepository = definitionsRepository;
        }

        public void UpdatePriceModificators(City city)
        {
            city.PriceModificators.Clear();
            var mod = (city.Owner != null && city.Owner.IsUserControlled) ? 20 : 50;

            // Price modificators for each item type in that city
            foreach (var itemType in Enum.GetValues<ItemType>())
            {
                city.PriceModificators.Add(itemType, GlobalUtils.Rand(mod));
            }
        }

        public TerrainType GetCityTerrainType(City city)
        {
            return TerrainType.Forest;
        }
    }
}