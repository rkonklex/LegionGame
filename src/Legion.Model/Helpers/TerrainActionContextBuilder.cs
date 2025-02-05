using System;
using System.Diagnostics;
using Legion.Model.Types;

namespace Legion.Model.Helpers
{
    public class TerrainActionContextBuilder
    {
        private readonly ITerrainHelper _terrainHelper;
        private Army _userArmy;
        private Army _enemyArmy;

        public TerrainActionContextBuilder(ITerrainHelper terrainHelper)
        {
            _terrainHelper = terrainHelper;
        }

        public void SetUserArmy(Army army, int zoneX, int zoneY, PlacementZone type)
        {
            Debug.Assert(army.IsUserControlled);
            _terrainHelper.PositionCharacters(army, zoneX, zoneY, type);
            _userArmy = army;
        }

        public void SetEnemyArmy(Army army, int zoneX, int zoneY, PlacementZone type)
        {
            Debug.Assert(!army.IsUserControlled);
            _terrainHelper.PositionCharacters(army, zoneX, zoneY, type);
            _enemyArmy = army;
        }

        public TerrainActionContext GetResult()
        {
            return new TerrainActionContext
            {
                UserArmy = _userArmy,
                EnemyArmy = _enemyArmy,
                Type = TerrainActionType.Battle,
            };
        }
    }
}