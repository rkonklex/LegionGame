using System.Collections.Generic;
using Legion.Model.Helpers;
using Legion.Model.Repositories;
using Legion.Model.Types;
using Legion.Utils;

namespace Legion.Model
{
    public class CityIncidents : ICityIncidents
    {
        private readonly IArmiesRepository _armiesRepository;
        private readonly ICharactersRepository _charactersRepository;
        private readonly IRaceDefinitionsRepository _raceDefinitionsRepository;
        private readonly ILegionInfo _legionInfo;
        private readonly IArmiesHelper _armiesHelper;
        private readonly IMessagesService _messagesService;
        private readonly IViewSwitcher _viewSwitcher;

        public CityIncidents(IArmiesRepository armiesRepository,
            ICharactersRepository charactersRepository,
            IRaceDefinitionsRepository raceDefinitionsRepository,
            ILegionInfo legionInfo,
            IArmiesHelper armiesHelper,
            IMessagesService messagesService,
            IViewSwitcher viewSwitcher)
        {
            _armiesRepository = armiesRepository;
            _charactersRepository = charactersRepository;
            _raceDefinitionsRepository = raceDefinitionsRepository;
            _legionInfo = legionInfo;
            _armiesHelper = armiesHelper;
            _messagesService = messagesService;
            _viewSwitcher = viewSwitcher;
        }

        public void Plague(City city, int type)
        {
            if (type == 0)
            {
                var newPopulation = city.Population - city.Population / 4;
                if (newPopulation >= 50) city.Population = newPopulation;
                city.Buildings.RemoveAll(_ => GlobalUtils.Rand(1) == 1);

                var fireMessage = new Message();
                fireMessage.Type = MessageType.FireInTheCity;
                fireMessage.MapObjects = new List<MapObject> { city };
                _messagesService.ShowMessage(fireMessage);
            }
            else if (type == 1)
            {
                var newPopulation = city.Population - city.Population / 2;
                if (newPopulation >= 50) city.Population = newPopulation;

                var epidemyMessage = new Message();
                epidemyMessage.Type = MessageType.EpidemyInTheCity;
                epidemyMessage.MapObjects = new List<MapObject> { city };
                _messagesService.ShowMessage(epidemyMessage);
            }
            else if (type == 2)
            {
                city.Food = 0;

                var ratsMessage = new Message();
                ratsMessage.Type = MessageType.RatsInTheCity;
                ratsMessage.MapObjects = new List<MapObject> { city };
                _messagesService.ShowMessage(ratsMessage);
            }
        }

        public void Riot(City city)
        {
            var userArmy = _armiesHelper.FindUserArmyInCity(city);
            if (userArmy == null)
            {
                city.Owner = null;
                city.Morale = 30;
                //TODO: //CENTER[MIASTA(M, 0, M_X), MIASTA(M, 0, M_Y), 1]
                var riotMessage = new Message();
                riotMessage.Type = MessageType.RiotInTheCity;
                riotMessage.MapObjects = new List<MapObject> { city };
                _messagesService.ShowMessage(riotMessage);
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
                var villager = _charactersRepository.CreateNpc(_raceDefinitionsRepository.Villager, _legionInfo.Power);
                rebelArmy.Characters.Add(villager);
            }

            //TODO: BITWA[_ATAK,40,1,1,0,1,1,1,TEREN,M]
            var battleContext = new TerrainActionContext();
            battleContext.UserArmy = userArmy;
            battleContext.EnemyArmy = rebelArmy;
            battleContext.Type = TerrainActionType.Battle;
            battleContext.ActionAfter = () =>
            {
                city.Population -= city.Population / 4;

                if (userArmy.IsKilled)
                {
                    city.Owner = null;
                    city.Morale = 30;

                    var defeatMessage = new Message();
                    defeatMessage.Type = MessageType.RiotInTheCityLost;
                    defeatMessage.MapObjects = new List<MapObject> { city };
                    _messagesService.ShowMessage(defeatMessage);
                }
                else
                {
                    city.Morale = 50;
                    city.Craziness += GlobalUtils.Rand(3) + 5;
                }
            };

            var defenceMessage = new Message();
            defenceMessage.Type = MessageType.RiotInTheCityWithDefence;
            defenceMessage.MapObjects = new List<MapObject> { city, userArmy };
            defenceMessage.OnClose = () =>
            {
                _viewSwitcher.OpenTerrain(battleContext);
            };
            _messagesService.ShowMessage(defenceMessage);
        }
    }
}