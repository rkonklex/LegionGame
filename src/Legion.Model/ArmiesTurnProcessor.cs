using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AwaitableCoroutine;
using Legion.Model.Helpers;
using Legion.Model.Repositories;
using Legion.Model.Types;
using Legion.Model.Types.Definitions;
using Legion.Utils;

namespace Legion.Model
{
    public class ArmiesTurnProcessor : IArmiesTurnProcessor
    {
        private readonly ILegionInfo _legionInfo;
        private readonly IArmiesRepository _armiesRepository;
        private readonly ICitiesRepository _citiesRepository;
        private readonly IPlayersRepository _playersRepository;
        private readonly IArmiesHelper _armiesHelper;
        private readonly IBattleManager _battleManager;
        private readonly IArmyActivities _armyActivities;
        private readonly IMessagesService _messagesService;

        public ArmiesTurnProcessor(ILegionInfo legionInfo,
            IArmiesRepository armiesRepository,
            ICitiesRepository citiesRepository,
            IPlayersRepository playersRepository,
            IArmiesHelper armiesHelper,
            IBattleManager battleManager,
            IArmyActivities armyIncidents,
            IMessagesService messagesService)
        {
            _legionInfo = legionInfo;
            _armiesRepository = armiesRepository;
            _citiesRepository = citiesRepository;
            _playersRepository = playersRepository;
            _armiesHelper = armiesHelper;
            _battleManager = battleManager;
            _armyActivities = armyIncidents;
            _messagesService = messagesService;
        }

        public async Coroutine NextTurn()
        {
            // Create a copy of the list to avoid modification during iteration
            var armiesSnapshot = _armiesRepository.Armies.ToArray();

            // Process user-controlled armies first
            foreach (var army in armiesSnapshot.Where(army => army.IsUserControlled))
            {
                await ProcessTurn(army);
            }

            // Process AI-controlled armies
            foreach (var army in armiesSnapshot.Where(army => !army.IsUserControlled))
            {
                await ProcessTurn(army);
            }
        }

        private async Coroutine ProcessTurn(Army army)
        {
            if (army.IsUserControlled)
            {
                UpdateCharactersExperience(army);

                var hasEnoughFood = CheckFoodStatus(army);
                if (!hasEnoughFood)
                {
                    // TODO: wait for message window to close before killing the army
                    _armiesRepository.KillArmy(army);
                    return;
                }
            }
            else
            {
                UpdateDaysToGetInfo(army);
            }

            await HandleArmyCurrentAction(army);
        }

        private void UpdateCharactersExperience(Army army)
        {
            Debug.Assert(army.IsUserControlled);
            if (_legionInfo.CurrentDay % 30 == 0)
            {
                foreach (var member in army.Characters)
                {
                    var raceType = member.Type as RaceDefinition;
                    var experience = member.Experience + GlobalUtils.Rand(raceType.Intelligence);
                    if (experience <= 95) member.Experience = experience;
                }
            }
        }

        private bool CheckFoodStatus(Army army)
        {
            Debug.Assert(army.IsUserControlled);
            var days = army.Food / army.Characters.Count;
            if (days < 5 && days > 0)
            {
                var message = new Message();
                message.Type = MessageType.ArmyLowOnFood;
                message.MapObjects = new List<MapObject> { army };
                _messagesService.ShowMessage(message);
            }
            else if (army.Food <= 0)
            {
                var message = new Message();
                message.Type = MessageType.ArmyDisbandedNoFood;
                message.MapObjects = new List<MapObject> { army };
                _messagesService.ShowMessage(message);
                return false;
            }
            return true;
        }

        private void UpdateDaysToGetInfo(Army army)
        {
            Debug.Assert(!army.IsUserControlled);
            if (army.DaysToGetInfo < 28 && army.DaysToGetInfo > 0)
            {
                army.DaysToGetInfo--;
            }
        }

        private async Coroutine HandleArmyCurrentAction(Army army)
        {
            switch (army.CurrentAction)
            {
                case ArmyActions.Move:
                case ArmyActions.FastMove:
                case ArmyActions.Attack:
                    if (!army.IsUserControlled && GlobalUtils.Rand(6) == 1)
                    {
                        GiveTheOrder(army);
                        return;
                    }
                    await Move(army);
                    break;
                case ArmyActions.Camping:
                    HandleCamping(army);
                    if (!army.IsUserControlled)
                    {
                        GiveTheOrder(army);
                    }
                    break;
                case ArmyActions.Hunting:
                    await HandleHunting(army);
                    break;
            }
        }

        private void HandleCamping(Army army)
        {
            if (army.IsUserControlled)
            {
                if (_armiesHelper.IsArmyInTheCity(army) == null)
                {
                    army.Food -= army.Characters.Count;
                }
            }

            foreach (var member in army.Characters)
            {
                var magic = member.Magic + GlobalUtils.Rand(5) + 5;
                if (magic <= member.MagicMax) member.Magic = magic;
                if (member.Energy < member.EnergyMax)
                {
                    var energy = member.Energy + GlobalUtils.Rand(20) + 10;
                    if (energy <= member.EnergyMax) member.Energy = energy;
                }
            }
        }

        private async Coroutine HandleHunting(Army army)
        {
            if (army.IsUserControlled)
            {
                if (_armiesHelper.IsArmyInTheCity(army) == null)
                {
                    army.Food -= army.Characters.Count;
                }

                var terrainType = _armiesHelper.GetArmyTerrainType(army);
                var huntRoll = terrainType switch
                {
                    TerrainType.Forest => 1,
                    TerrainType.Steppe => 2,
                    TerrainType.Desert => 6,
                    TerrainType.Rocks => 5,
                    TerrainType.Snow => 4,
                    TerrainType.Swamp => 3,
                    _ => 0,
                };

                if (GlobalUtils.Rand(huntRoll) == 1)
                {
                    await _armyActivities.Hunt(army, terrainType);
                }
            }
        }

        private void GiveTheOrder(Army army)
        {
            var oldDistance = 120;

            foreach (var city in _citiesRepository.Cities)
            {
                var craziness = 0;
                if (city.Owner == null || city.IsUserControlled)
                {
                    craziness = 300 - _legionInfo.Power;
                }
                else
                {
                    craziness = 2200 + _legionInfo.Power;
                }
                if (army.IsChaosControlled)
                {
                    craziness = 1;
                }
                if (GlobalUtils.Rand(craziness) == 1)
                {
                    army.Owner.UpdateWar(city.Owner, GlobalUtils.Rand(20) + 8);
                    if (city.Owner != null)
                    {
                        city.Owner.UpdateWar(army.Owner, GlobalUtils.Rand(20) + 8);
                    }
                }

                if (army.Owner.GetWar(city.Owner) > 0)
                {
                    if (army.IsChaosControlled && city.Population < 200) continue;
                    var distance = Distance(army, city);
                    if (distance < oldDistance)
                    {
                        army.CurrentAction = ArmyActions.Attack;
                        army.Target = city;
                        oldDistance = distance;
                    }
                }
            }

            foreach (var otherArmy in _armiesRepository.Armies)
            {
                if (army == otherArmy) continue;
                if (army.Owner.GetWar(otherArmy.Owner) > 0)
                {
                    var city = _armiesHelper.IsArmyInTheCity(otherArmy);
                    if (city != null && city.Owner == otherArmy.Owner)
                    {
                        var distance = Distance(army, city);
                        if (distance < oldDistance)
                        {
                            army.CurrentAction = ArmyActions.Attack;
                            army.Target = city;
                            oldDistance = distance;
                        }
                    }
                    else
                    {
                        var distance = Distance(army, otherArmy);
                        if (distance < oldDistance)
                        {
                            army.CurrentAction = ArmyActions.Attack;
                            army.Target = otherArmy;
                            oldDistance = distance;
                        }
                    }
                }
            }
        }

        private int Distance(MapObject obj1, MapObject obj2)
        {
            var dx = obj2.X - obj1.X;
            var dy = obj2.Y - obj1.Y;
            var dist = Math.Abs(Math.Sqrt(dx * dx + dy * dy));
            return (int)dist;

        }

        private bool IsArrived(Army army)
        {
            return Math.Abs(army.Target.X - army.X) < 3 && Math.Abs(army.Target.Y - army.Y) < 3;
        }

        private async Coroutine Move(Army army)
        {
            if (army.Target == null || army.Target.Owner == army.Owner)
            {
                army.CurrentAction = ArmyActions.Camping;
                return;
            }

            var foodConsumption = 1;
            var speed = army.Speed;

            if (army.CurrentAction == ArmyActions.FastMove)
            {
                foodConsumption = 3;
                speed = army.Speed * 2;
            }

            if (army.Owner == _playersRepository.UserPlayer)
            {
                army.Food -= army.Characters.Count * foodConsumption;
                if (army.Food < 0) army.Food = 0;
            }

            await DoArmyMovement(army, speed);

            if (army.IsUserControlled)
            {
                var visitedCity = _armiesHelper.IsArmyInTheCity(army);
                if (visitedCity != null)
                {
                    visitedCity.DaysToGetInfo = 0;
                }
            }

            if (army.IsTracked)
            {
                //TODO: CENTER[X1,Y1,1]
            }

            // target could be destroyed meanwhile
            if (army.Target != null && IsArrived(army))
            {
                Debug.Assert(army.CurrentAction == ArmyActions.Attack);
                army.CurrentAction = ArmyActions.Camping;

                switch (army.Target.Type)
                {
                    case MapObjectType.Adventure:
                        HandleAdventure(army);
                        break;
                    case MapObjectType.Army:
                        await _battleManager.AttackOnArmy(army, (Army)army.Target);
                        break;
                    case MapObjectType.City:
                        await _battleManager.AttackOnCity(army, (City)army.Target);
                        break;
                }
            }
            else if (army.IsUserControlled)
            {
                // not planned adventure can happen meantime
                await HandleEncounters(army);
            }

            //BUSY_ANIM
        }

        private async Coroutine DoArmyMovement(Army army, int speed)
        {
            var dx = army.Target.X - army.X;
            var dy = army.Target.Y - army.Y;

            var l = MathF.Sqrt(dx * dx + dy * dy) + 0.2f;
            var vx = dx / l;
            var vy = dy / l;

            var x1 = (float)army.X;
            var y1 = (float)army.Y;
            for (var i = 0; i <= speed && !IsArrived(army); i++)
            {
                x1 += vx;
                y1 += vy;

                army.X = (int)x1;
                army.Y = (int)y1;
                await Coroutine.Yield();
            }
        }

        private async Coroutine HandleEncounters(Army army)
        {
            Debug.Assert(army.IsUserControlled);
            EncounterType? encounterType = null;
            var terrainType = _armiesHelper.GetArmyTerrainType(army);
            switch (terrainType)
            {
                case TerrainType.Swamp:
                    if (GlobalUtils.Rand(3) == 1)
                        encounterType = EncounterType.StuckInSwamp;
                    break;

                case TerrainType.Snow:
                    if (GlobalUtils.Rand(5) == 1)
                        encounterType = EncounterType.RabidWolves;
                    break;

                case TerrainType.Forest:
                    switch (GlobalUtils.Rand(45))
                    {
                        case 0:
                            encounterType = EncounterType.ForestTrolls;
                            break;
                        case 1:
                            encounterType = EncounterType.Gargoyl;
                            break;
                        case 2:
                            encounterType = EncounterType.LoneKnight;
                            break;
                        case 3:
                            encounterType = EncounterType.CaveEntrance;
                            break;
                        case 4:
                            encounterType = EncounterType.Bandits;
                            break;
                    }
                    break;

                case TerrainType.Steppe:
                    switch (GlobalUtils.Rand(45))
                    {
                        case 1:
                            encounterType = EncounterType.Bandits;
                            break;
                        case 2:
                            encounterType = EncounterType.LoneKnight;
                            break;
                    }
                    break;

                case TerrainType.Desert:
                    switch (GlobalUtils.Rand(45))
                    {
                        case 1:
                            encounterType = EncounterType.Bandits;
                            break;
                    }
                    break;

                case TerrainType.Rocks:
                    switch (GlobalUtils.Rand(45))
                    {
                        case 1:
                            encounterType = EncounterType.Bandits;
                            break;
                        case 5:
                            encounterType = EncounterType.CaveEntrance;
                            break;
                    }
                    break;
            }

            if (encounterType.HasValue)
            {
                await _armyActivities.Encounter(army, encounterType.Value);
            }
        }

        private bool HandleAdventure(Army army)
        {
            //NOTE: temporary return true as we didn't implemented this yet:
            return true;
            //NOTE: but should return false  - because we will go into the Action scenery
            /* 
                      NR=LOK-121
                      LOK=PRZYGODY(NR,P_TEREN)
                      CENTER[X1,Y1,1]
                      MA_PRZYGODA[A,NR]
                      'nie chcę już więcej przygód 
                      SKIP=1
                    */
        }
    }
}