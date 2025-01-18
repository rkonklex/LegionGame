using System.Collections.Generic;
using AwaitableCoroutine;
using Legion.Model.Helpers;
using Legion.Model.Repositories;
using Legion.Model.Types;
using Legion.Utils;

namespace Legion.Model
{
    public class CityIncidents : ICityIncidents
    {
        private readonly IArmiesRepository _armiesRepository;
        private readonly ICharacterDefinitionsRepository _characterDefinitionsRepository;
        private readonly ILegionInfo _legionInfo;
        private readonly IArmiesHelper _armiesHelper;
        private readonly IMessagesService _messagesService;
        private readonly IViewSwitcher _viewSwitcher;

        public CityIncidents(IArmiesRepository armiesRepository,
            ICharacterDefinitionsRepository characterDefinitionsRepository,
            ILegionInfo legionInfo,
            IArmiesHelper armiesHelper,
            IMessagesService messagesService,
            IViewSwitcher viewSwitcher)
        {
            _armiesRepository = armiesRepository;
            _characterDefinitionsRepository = characterDefinitionsRepository;
            _legionInfo = legionInfo;
            _armiesHelper = armiesHelper;
            _messagesService = messagesService;
            _viewSwitcher = viewSwitcher;
        }

        public async Coroutine Plague(City city, int type)
        {
            if (type == 0)
            {
                var newPopulation = city.Population - city.Population / 4;
                if (newPopulation >= 50) city.Population = newPopulation;
                city.Buildings.RemoveAll(_ => GlobalUtils.Rand(1) == 1);

                await _messagesService.ShowMessageAsync(MessageType.FireInTheCity, city);
            }
            else if (type == 1)
            {
                var newPopulation = city.Population - city.Population / 2;
                if (newPopulation >= 50) city.Population = newPopulation;

                await _messagesService.ShowMessageAsync(MessageType.EpidemyInTheCity, city);
            }
            else if (type == 2)
            {
                city.Food = 0;

                await _messagesService.ShowMessageAsync(MessageType.RatsInTheCity, city);
            }
        }

        public async Coroutine Riot(City city)
        {
            var userArmy = _armiesHelper.FindUserArmyInCity(city);
            if (userArmy == null)
            {
                city.Owner = null;
                city.Morale = 30;
                //TODO: //CENTER[MIASTA(M, 0, M_X), MIASTA(M, 0, M_Y), 1]
                await _messagesService.ShowMessageAsync(MessageType.RiotInTheCity, city);
                return;
            }

            //TODO: CENTER[MIASTA(M, 0, M_X), MIASTA(M, 0, M_Y), 1]

            // there is user army in city and can fight with rebels
            var villagersCount = 2 + GlobalUtils.Rand(2);
            var count = (city.Population / 70) + 1;
            if (count > 10) count = 10;
            count -= villagersCount;

            var rebelArmy = _armiesRepository.CreateTempArmy(count, _legionInfo.Power);
            //'wieśniacy wśród buntowników 
            for (var i = 0; i < villagersCount; i++)
            {
                _armiesRepository.AddNpc(rebelArmy, _legionInfo.Power, _characterDefinitionsRepository.Villager);
            }

            //TODO: BITWA[_ATAK,40,1,1,0,1,1,1,TEREN,M]
            await _messagesService.ShowMessageAsync(MessageType.RiotInTheCityWithDefence, city, userArmy);

            var battleContext = new TerrainActionContext();
            battleContext.UserArmy = userArmy;
            battleContext.EnemyArmy = rebelArmy;
            battleContext.Type = TerrainActionType.Battle;

            _viewSwitcher.OpenTerrain(battleContext);
            await battleContext.ActionFinished;

            city.Population -= city.Population / 4;

            if (userArmy.IsKilled)
            {
                city.Owner = null;
                city.Morale = 30;

                await _messagesService.ShowMessageAsync(MessageType.RiotInTheCityLost, city);
            }
            else
            {
                city.Morale = 50;
                city.Craziness += GlobalUtils.Rand(3) + 5;
            }
        }
    }
}