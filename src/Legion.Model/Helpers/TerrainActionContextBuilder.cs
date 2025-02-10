using System;
using System.Diagnostics;
using System.Linq;
using Legion.Model.Types;

namespace Legion.Model.Helpers
{
    public class TerrainActionContextBuilder
    {
        private readonly ITerrainHelper _terrainHelper;
        private Scenery _scenery;
        private Army _userArmy;
        private Army _enemyArmy;

        public TerrainActionContextBuilder(ITerrainHelper terrainHelper)
        {
            _terrainHelper = terrainHelper;
        }

        public void SetScenery(TerrainType terrainType, City city = null)
        {
            _scenery = _terrainHelper.CreateScenery(GetSceneryType(terrainType), city);
        }

        private static SceneryType GetSceneryType(TerrainType terrainType) => terrainType switch
        {
            TerrainType.Forest => SceneryType.Forest,
            TerrainType.Steppe => SceneryType.Steppe,
            TerrainType.Desert => SceneryType.Desert,
            TerrainType.Rocks => SceneryType.Rocks,
            TerrainType.Snow => SceneryType.Snow,
            TerrainType.Swamp => SceneryType.Swamp,
            _ => throw new ArgumentOutOfRangeException(nameof(terrainType), terrainType, "Unknown terrain type"),
        };

        public void SetUserArmy(Army army, int zoneX, int zoneY, PlacementZone type)
        {
            Debug.Assert(army.IsUserControlled);
            _terrainHelper.PositionCharacters(army, zoneX, zoneY, type, _scenery.Obstacles);
            _userArmy = army;
        }

        public void SetEnemyArmy(Army army, int zoneX, int zoneY, PlacementZone type)
        {
            Debug.Assert(!army.IsUserControlled);
            var obstacles = Enumerable.Concat<TerrainObject>(_scenery.Obstacles, _userArmy.Characters);
            _terrainHelper.PositionCharacters(army, zoneX, zoneY, type, obstacles);
            _enemyArmy = army;
        }

        public TerrainActionContext GetResult()
        {
            return new TerrainActionContext
            {
                Scenery = _scenery,
                UserArmy = _userArmy,
                EnemyArmy = _enemyArmy,
                Type = TerrainActionType.Battle,
            };
        }
    }
}