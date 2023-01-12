using System.Collections.Generic;
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
        private readonly IMessagesService _messagesService;
        private readonly IViewSwitcher _viewSwitcher;

        public BattleManager(IArmiesRepository armiesRepository,
            IPlayersRepository playersRepository,
            IArmiesHelper armiesHelper,
            ICitiesHelper citiesHelper,
            IMessagesService messagesService,
            IViewSwitcher viewSwitcher)
        {
            _playersRepository = playersRepository;
            _armiesRepository = armiesRepository;
            _armiesHelper = armiesHelper;
            _citiesHelper = citiesHelper;
            _messagesService = messagesService;
            _viewSwitcher = viewSwitcher;
        }

        public void AttackOnArmy(Army army, Army targetArmy)
        {
            if (!army.IsUserControlled && !targetArmy.IsUserControlled)
            {
                var loser = SimulatedBattle(army, targetArmy);
                if (loser.IsTracked)
                {
                    var message = new Message();
                    message.Type = MessageType.ArmyDestroyed;
                    message.MapObjects = new List<MapObject> { loser };
                    _messagesService.ShowMessage(message);
                }
            }
            else
            {
                var battleContext = new TerrainActionContext();
                battleContext.Type = TerrainActionType.Battle;

                var battleMessage = new Message();

                if (army.IsUserControlled)
                {
                    battleContext.UserArmy = army;
                    battleContext.EnemyArmy = targetArmy;

                    var days = GlobalUtils.Rand(30) + 10;
                    army.Owner.UpdateWar(targetArmy.Owner, days);
                    targetArmy.Owner.UpdateWar(army.Owner, days);

                    targetArmy.DaysToGetInfo = 0;

                    battleMessage.Type = MessageType.UserAttacksArmy;
                    battleMessage.MapObjects = new List<MapObject> { targetArmy };
                }
                else
                {
                    battleContext.UserArmy = targetArmy;
                    battleContext.EnemyArmy = army;

                    army.DaysToGetInfo = 0;

                    battleMessage.Type = MessageType.EnemyAttacksUserArmy;
                    battleMessage.MapObjects = new List<MapObject> { army };
                }

                battleContext.ActionAfter = () =>
                {
                    if (army.IsKilled)
                    {
                        var message = new Message();
                        message.Type = MessageType.ArmyDestroyed;
                        message.MapObjects = new List<MapObject> { army };
                        _messagesService.ShowMessage(message);
                    }
                    if (targetArmy.IsKilled)
                    {
                        var message = new Message();
                        message.Type = MessageType.ArmyDestroyed;
                        message.MapObjects = new List<MapObject> { targetArmy };
                        _messagesService.ShowMessage(message);
                    }
                };

                battleMessage.OnClose = () => { _viewSwitcher.OpenTerrain(battleContext); };
                _messagesService.ShowMessage(battleMessage);
            }
        }

        public void AttackOnCity(Army army, City city)
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

                cityArmy = _armiesRepository.CreateTempArmy(defendersCount);
            }

            if (!army.IsUserControlled && !city.IsUserControlled)
            {
                SimulatedBattle(army, cityArmy);
                AfterAttackOnCity(army, city, cityArmy);
            }
            else
            {
                if (cityArmy != null)
                {
                    //TODO: CENTER[X1,Y1,1]

                    var battleContext = new TerrainActionContext();
                    battleContext.Type = TerrainActionType.Battle;
                    battleContext.ActionAfter = () => AfterAttackOnCity(army, city, cityArmy);

                    var battleMessage = new Message();

                    if (city.IsUserControlled)
                    {
                        battleContext.UserArmy = cityArmy;
                        battleContext.EnemyArmy = army;

                        battleMessage.Type = MessageType.EnemyAttacksUserCity;
                        battleMessage.MapObjects = new List<MapObject> { city, army };
                    }
                    if (army.IsUserControlled)
                    {
                        battleContext.UserArmy = army;
                        battleContext.EnemyArmy = cityArmy;

                        var days = GlobalUtils.Rand(30) + 10;
                        army.Owner.UpdateWar(city.Owner, days);
                        if (city.Owner != null) { city.Owner.UpdateWar(army.Owner, days); }

                        battleMessage.Type = MessageType.UserAttackCity;
                        battleMessage.MapObjects = new List<MapObject> { city, army };
                    }

                    battleMessage.OnClose = () => { _viewSwitcher.OpenTerrain(battleContext); };
                    _messagesService.ShowMessage(battleMessage);
                }
                else
                {
                    // city was taken over without battle
                    AfterAttackOnCity(army, city, null);
                }
            }
        }

        private void AfterAttackOnCity(Army army, City city, Army cityArmy)
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

                    var burnedCityMessage = new Message();
                    burnedCityMessage.Type = MessageType.ChaosWarriorsBurnedCity;
                    burnedCityMessage.MapObjects = new List<MapObject> { city };
                    _messagesService.ShowMessage(burnedCityMessage);
                }
                else
                {
                    var wasCityUserControlled = city.IsUserControlled;
                    city.Owner = army.Owner;

                    if (wasCityUserControlled || army.IsUserControlled || army.IsTracked)
                    {
                        var capturedCityMessage = new Message();
                        capturedCityMessage.MapObjects = new List<MapObject> { city };

                        if (army.IsUserControlled)
                        {
                            _citiesHelper.UpdatePriceModificators(city);
                            capturedCityMessage.Type = MessageType.UserCapturedCity;
                        }
                        else if (wasCityUserControlled)
                        {
                            capturedCityMessage.Type = cityArmy switch
                            {
                                null => MessageType.EnemyCapturedUserCity,
                                _ => MessageType.EnemyConqueredUserCity,
                            };
                        }
                        else // army.IsTracked
                        {
                            capturedCityMessage.Type = MessageType.EnemyConqueredCity;
                        }

                        _messagesService.ShowMessage(capturedCityMessage);
                    }
                }
            }
            else if (army.IsKilled)
            {
                if (army.IsTracked || army.IsUserControlled)
                {
                    //TODO: CENTER[X1, Y1, 1]
                    var failedMessage = new Message();
                    failedMessage.Type = MessageType.ArmyDestroyedCapturingCity;
                    failedMessage.MapObjects = new List<MapObject> { army, city };
                    _messagesService.ShowMessage(failedMessage);
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