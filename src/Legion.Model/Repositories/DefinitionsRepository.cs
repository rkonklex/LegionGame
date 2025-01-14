using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Legion.Model.Types.Definitions;
using Newtonsoft.Json;

namespace Legion.Model.Repositories
{
    public class DefinitionsRepository : IDefinitionsRepository
    {
        private static readonly string FilePath = Path.Combine("data", "model.json");
        private DefinitionsModel _model;

        public DefinitionsRepository()
        {
            Load();
        }

        private void Load()
        {
            var modelJson = File.ReadAllText(FilePath);

            _model = JsonConvert.DeserializeObject<DefinitionsModel>(modelJson);
            if (_model == null)
            {
                throw new Exception("Unable to load main game model!");
            }
        }

        public List<BuildingDefinition> Buildings => _model.Buildings;

        public List<ItemDefinition> Items => _model.Items;

        public List<CreatureDefinition> Creatures => _model.Creatures;

        public List<RaceDefinition> Races => _model.Races;

        public ItemDefinition GetItemByOldIndex(int oid)
        {
            return Items.FirstOrDefault(m => m.Oid == oid);
        }
    }
}