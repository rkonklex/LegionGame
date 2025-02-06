using System;
using System.Collections.Generic;
using AwaitableCoroutine;
using Legion.Model.Helpers;
using Legion.Model.Repositories;
using Legion.Model.Types;
using Legion.Utils;

namespace Legion.Model
{
    public class BattleManager : IBattleManager
    {
        private readonly IArmiesRepository _armiesRepository;
        private readonly IPlayersRepository _playersRepository;
        private readonly IArmiesHelper _armiesHelper;
        private readonly ICitiesHelper _citiesHelper;
        private readonly ITerrainHelper _terrainHelper;
        private readonly IMessagesService _messagesService;
        private readonly IViewSwitcher _viewSwitcher;

        public BattleManager(IArmiesRepository armiesRepository,
            IPlayersRepository playersRepository,
            IArmiesHelper armiesHelper,
            ICitiesHelper citiesHelper,
            ITerrainHelper terrainHelper,
            IMessagesService messagesService,
            IViewSwitcher viewSwitcher)
        {
            _playersRepository = playersRepository;
            _armiesRepository = armiesRepository;
            _armiesHelper = armiesHelper;
            _citiesHelper = citiesHelper;
            _terrainHelper = terrainHelper;
            _messagesService = messagesService;
            _viewSwitcher = viewSwitcher;
        }

        public async Coroutine AttackOnArmy(Army army, Army targetArmy, WorldDirection movementDirection)
        {
            if (!army.IsUserControlled && !targetArmy.IsUserControlled)
            {
                var loser = SimulatedBattle(army, targetArmy);
                if (loser.IsTracked)
                {
                    await _messagesService.ShowMessageAsync(MessageType.ArmyDestroyed, loser);
                }
            }
            else
            {
                var targetCity = _armiesHelper.IsArmyInTheCity(targetArmy);
                var terrainType = targetCity != null ? _citiesHelper.GetCityTerrainType(targetCity) : _armiesHelper.GetArmyTerrainType(targetArmy);

                var placementZone = movementDirection switch
                {
                    WorldDirection.East or WorldDirection.West => PlacementZone.RandomY,
                    WorldDirection.North or WorldDirection.South => PlacementZone.RandomX,
                    _ => throw new ArgumentOutOfRangeException(nameof(movementDirection), "Invalid movement direction"),
                };
                var (xw1, xw2) = movementDirection switch
                {
                    WorldDirection.West => (0, 2),
                    WorldDirection.East => (2, 0),
                    _ => (1, 1),
                };
                var (yw1, yw2) = movementDirection switch
                {
                    WorldDirection.South => (0, 2),
                    WorldDirection.North => (2, 0),
                    _ => (1, 1),
                };

                if (army.IsUserControlled)
                {
                    var days = GlobalUtils.Rand(30) + 10;
                    army.Owner.UpdateWar(targetArmy.Owner, days);
                    targetArmy.Owner.UpdateWar(army.Owner, days);

                    targetArmy.DaysToGetInfo = 0;

                    await _messagesService.ShowMessageAsync(MessageType.UserAttacksArmy, army, targetArmy);

                    var builder = _terrainHelper.BuildTerrainActionContext();
                    builder.SetScenery(terrainType, targetCity);
                    builder.SetUserArmy(army, xw1, yw1, placementZone);
                    builder.SetEnemyArmy(targetArmy, xw2, yw2, placementZone);
                    await _viewSwitcher.OpenTerrainAsync(builder.GetResult());
                }
                else
                {
                    army.DaysToGetInfo = 0;

                    await _messagesService.ShowMessageAsync(MessageType.EnemyAttacksUserArmy, army, targetArmy);

                    var builder = _terrainHelper.BuildTerrainActionContext();
                    builder.SetScenery(terrainType, targetCity);
                    builder.SetUserArmy(army, xw1, yw1, placementZone);
                    builder.SetEnemyArmy(targetArmy, xw2, yw2, placementZone);
                    await _viewSwitcher.OpenTerrainAsync(builder.GetResult());
                }

                if (army.IsKilled)
                {
                    await _messagesService.ShowMessageAsync(MessageType.ArmyDestroyed, army);
                }
                if (targetArmy.IsKilled)
                {
                    await _messagesService.ShowMessageAsync(MessageType.ArmyDestroyed, targetArmy);
                }
            }
        }

        public async Coroutine AttackOnCity(Army army, City city)
        {
            //var wall = 0;

            //if (city.WallType == 0 || army.Owner.IsChaosControlled)
            //{
            //	wall = city.Id;
            //}
            //else
            //{
            //	wall = -city.WallType - 1;
            //}

            Army cityArmy = null;
            if (city.IsUserControlled)
            {
                cityArmy = _armiesHelper.FindUserArmyInCity(city);
            }
            else
            {
                var defendersCount = (city.Population / 70) + 1;
                if (defendersCount > 10) defendersCount = 10;

                var defencePower = (city.Morale / 3) + 10;
                cityArmy = _armiesRepository.CreateTempArmy(defendersCount, defencePower);
            }

            if (!army.IsUserControlled && !city.IsUserControlled)
            {
                SimulatedBattle(army, cityArmy);
                await AfterAttackOnCity(army, city, cityArmy);
            }
            else
            {
                if (cityArmy != null)
                {
                    //TODO: CENTER[X1,Y1,1]
                    var terrainType = _citiesHelper.GetCityTerrainType(city);

                    if (city.IsUserControlled)
                    {
                        await _messagesService.ShowMessageAsync(MessageType.EnemyAttacksUserCity, army, city);

                        // BITWA[OBRONA,A,0,0,2,0,2,2,TEREN,MUR]
                        var builder = _terrainHelper.BuildTerrainActionContext();
                        builder.SetScenery(terrainType);
                        builder.SetUserArmy(cityArmy, 0, 0, PlacementZone.RandomX);
                        builder.SetEnemyArmy(army, 0, 2, PlacementZone.RandomX);
                        await _viewSwitcher.OpenTerrainAsync(builder.GetResult());
                    }
                    else if (army.IsUserControlled)
                    {
                        var days = GlobalUtils.Rand(30) + 10;
                        army.Owner.UpdateWar(city.Owner, days);
                        if (city.Owner != null) { city.Owner.UpdateWar(army.Owner, days); }

                        await _messagesService.ShowMessageAsync(MessageType.UserAttackCity, army, city);

                        // BITWA[A,40,0,2,2,0,0,2,TEREN,MUR]
                        var builder = _terrainHelper.BuildTerrainActionContext();
                        builder.SetScenery(terrainType, city);
                        builder.SetUserArmy(army, 0, 2, PlacementZone.RandomX);
                        builder.SetEnemyArmy(cityArmy, 0, 0, PlacementZone.RandomX);
                        await _viewSwitcher.OpenTerrainAsync(builder.GetResult());
                    }

                    await AfterAttackOnCity(army, city, cityArmy);
                }
                else
                {
                    // city was taken over without battle
                    await AfterAttackOnCity(army, city, null);
                }
            }
        }

        private async Coroutine AfterAttackOnCity(Army army, City city, Army cityArmy)
        {
            var oldPopulation = city.Population;
            // city's population is reduced if there was a battle
            if (cityArmy != null)
            {
                var populationAfterAttack = city.Population - oldPopulation / 4;
                if (populationAfterAttack >= 20) city.Population = populationAfterAttack;
            }

            if (cityArmy == null || cityArmy.IsKilled)
            {
                if (army.IsChaosControlled)
                {
                    city.Owner = null;
                    var populationAfterChaos = city.Population - oldPopulation / 2;
                    if (populationAfterChaos >= 20) city.Population = populationAfterChaos;
                    city.Buildings.RemoveAll(_ => GlobalUtils.Rand(2) == 1);

                    await _messagesService.ShowMessageAsync(MessageType.ChaosWarriorsBurnedCity, city);
                }
                else
                {
                    var wasCityUserControlled = city.IsUserControlled;
                    city.Owner = army.Owner;

                    MessageType? messageType = null;
                    if (army.IsUserControlled)
                    {
                        _citiesHelper.UpdatePriceModificators(city);
                        messageType = MessageType.UserCapturedCity;
                    }
                    else if (wasCityUserControlled)
                    {
                        messageType = cityArmy switch
                        {
                            null => MessageType.EnemyCapturedUserCity,
                            _ => MessageType.EnemyConqueredUserCity,
                        };
                    }
                    else if (army.IsTracked)
                    {
                        messageType = MessageType.EnemyConqueredCity;
                    }

                    if (messageType.HasValue)
                    {
                        await _messagesService.ShowMessageAsync(messageType.Value, army, city);
                    }
                }
            }
            else if (army.IsKilled)
            {
                if (army.IsTracked || army.IsUserControlled)
                {
                    //TODO: CENTER[X1, Y1, 1]
                    await _messagesService.ShowMessageAsync(MessageType.ArmyDestroyedCapturingCity, army, city);
                }
            }
        }

        public Army SimulatedBattle(Army a, Army b)
        {
            // //TODO: temporary things:
            // actionsManager.NextAction = new ActionInfo
            // {
            //     UserArmy = a,
            //     EnemyArmy = b,
            //     Type = ActionType.Battle
            // };
            // return;

            Army winner;
            Army loser;

            var s1 = a.Strength + GlobalUtils.Rand(100);
            var s2 = b.Strength + GlobalUtils.Rand(100);
            var s3 = 0;

            var ds = s1 - s2;
            if (ds >= 0)
            {
                winner = a;
                loser = b;
            }
            else
            {
                winner = b;
                loser = a;
            }
            s3 = s2 / 15;

            _armiesRepository.KillArmy(loser);

            foreach (var character in winner.Characters)
            {
                character.Energy -= s3;
            }
            winner.Characters.RemoveAll(m => m.IsKilled);

            return loser;
        }

        public void Battle(Army a, Army b)
        {
            /*
            Procedure BITWA[A,B,X1,Y1,T1,X2,Y2,T2,SCENERIA,WIES]
   ARM=A : WRG=B
   PL2=ARMIA(B,0,TMAG)
   Sprite Off 2
   SETUP["","Bitwa",""]
   If ARMIA(B,0,TMAG)=5
      For I=1 To 16
         Del Bob POTWORY+1
      Next I
      _LOAD[KAT$+"dane/potwory/szkielet","dane:potwory/szkielet","Dane",1]
      _LOAD[KAT$+"dane/potwory/szkielet.snd","dane:potwory/szkielet.snd","Dane",9]
   End If 
   RYSUJ_SCENERIE[SCENERIA,WIES]
   AGRESJA=ARMIA(B,0,TKORP)
   For I=1 To 10 : ARMIA(WRG,I,TKORP)=AGRESJA : Next I
   USTAW_WOJSKO[A,X1,Y1,T1]
   USTAW_WOJSKO[B,X2,Y2,T2]
   MAIN_ACTION
   If TESTING Then Pop Proc
   SETUP0
   VISUAL_OBJECTS
   Sprite 2,SPX,SPY,1
   
End Proc*/
        }
    }
}