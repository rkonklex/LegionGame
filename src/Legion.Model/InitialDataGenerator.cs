using System.Collections.Generic;
using System.Linq;
using Legion.Model.Helpers;
using Legion.Model.Repositories;
using Legion.Model.Types;
using Legion.Utils;

namespace Legion.Model
{
    public class InitialDataGenerator : IInitialDataGenerator
    {
        private readonly ILegionConfig _legionConfig;
        private readonly ILegionInfo _legionInfo;
        private readonly IDefinitionsRepository _definitionsRepository;
        private readonly IArmiesRepository _armiesRepository;
        private readonly IPlayersRepository _playersRepository;
        private readonly ICitiesRepository _citiesRepository;
        private readonly ICitiesHelper _citiesHelper;

        public InitialDataGenerator(
            ILegionConfig legionConfig,
            ILegionInfo legionInfo,
            IDefinitionsRepository definitionsRepository,
            IArmiesRepository armiesRepository,
            IPlayersRepository playersRepository,
            ICitiesRepository citiesRepository,
            ICitiesHelper citiesHelper)
        {
            _legionConfig = legionConfig;
            _legionInfo = legionInfo;
            _definitionsRepository = definitionsRepository;
            _armiesRepository = armiesRepository;
            _playersRepository = playersRepository;
            _citiesRepository = citiesRepository;
            _citiesHelper = citiesHelper;
        }

        public void Generate()
        {
            _legionInfo.CurrentDay = 1;
            _legionInfo.Power = 5;

            GeneratePlayers();
            GenerateCities();
            GenerateArmies();
        }

        private void GeneratePlayers()
        {
            //TODO: currently generate random player names for now, should be provided by user on new game
            for (var i = 1; i <= _legionConfig.PlayersCount; i++)
            {
                _playersRepository.Players.Add(new Player { Id = i, Money = 5000, Name = NamesGenerator.Generate() });
            }

            _playersRepository.UserPlayer = _playersRepository.Players.FirstOrDefault();
            _playersRepository.ChaosPlayer = _playersRepository.Players.LastOrDefault();
        }

        private void GenerateArmies()
        {
            const int NumArmiesPerRival = 3; // TODO: magic number

            foreach (var owner in _playersRepository.Players.Where(p => p.IsRivalControlled))
            {
                var xg = GlobalUtils.Rand(_legionConfig.WorldWidth - 200) + 100;
                var yg = GlobalUtils.Rand(_legionConfig.WorldHeight - 200) + 100;

                for (var k = 0; k < NumArmiesPerRival; k++)
                {
                    var army = _armiesRepository.CreateNpcArmy(owner, 10, _legionInfo.Power);
                    army.X = xg + GlobalUtils.Rand(200) - 100;
                    army.Y = yg + GlobalUtils.Rand(200) - 100;
                }
            }

            var ownArmy = _armiesRepository.CreateUserArmy(_playersRepository.UserPlayer, 5);
            ownArmy.X = GlobalUtils.Rand(_legionConfig.WorldWidth) + 20;
            ownArmy.Y = GlobalUtils.Rand(_legionConfig.WorldHeight) + 10;
            ownArmy.Food = 100;
        }

        private City GenerateCity(Player owner)
        {
            var city = new City();
            city.Name = NamesGenerator.Generate();

            do
            {
                //city.X = GlobalUtils.Rand(config.WorldWidth - 50) + 20; // X=Rnd(590)+20
                //city.Y = GlobalUtils.Rand(config.WorldHeight - 52) + 20; // Y=Rnd(460)+20

                city.X = GlobalUtils.Rand(_legionConfig.WorldWidth);
                city.Y = GlobalUtils.Rand(_legionConfig.WorldHeight);
            } while (!IsCityPositionAvailable(city.X, city.Y));

            city.Population = GlobalUtils.Rand(900) + 10;
            city.Owner = owner;
            city.BobId = 8 + (city.Owner != null ? city.Owner.Id : 0) * 2;
            if (city.Population > 700)
            {
                city.BobId++;
                city.WallType = GlobalUtils.Rand(2) + 1;
            }
            else
            {
                city.WallType = GlobalUtils.Rand(1);
            }

            city.Tax = GlobalUtils.Rand(25);
            city.Morale = GlobalUtils.Rand(100);
            //TEREN[X+4,Y+4]
            //city.TerrainType = terrainManager.GetTerrain(city.X + 4, city.Y + 4);
            //if (city.TerrainType == 7) city.TerrainType = 1;

            city.X += 8;
            city.Y += 8;
            city.Craziness = GlobalUtils.Rand(10) + 5;
            city.DaysToGetInfo = 30;

            _citiesHelper.UpdatePriceModificators(city);

            var buildings = GenerateBuildings();
            city.Buildings = buildings;

            return city;
        }

        private void GenerateCities()
        {
            const int NumCitiesPerRival = 2; // TODO: magic number
            var rivalPlayers = _playersRepository.Players.Where(p => p.IsRivalControlled);
            var numNeutralCities = _legionConfig.MaxCitiesCount - NumCitiesPerRival * rivalPlayers.Count();

            for (var i = 0; i < numNeutralCities; i++)
            {
                var city = GenerateCity(null);
                _citiesRepository.Cities.Add(city);
            }

            foreach (var owner in rivalPlayers)
            {
                for (var i = 0; i < NumCitiesPerRival; i++)
                {
                    var city = GenerateCity(owner);
                    _citiesRepository.Cities.Add(city);
                }
            }
        }

        private bool IsCityPositionAvailable(int x, int y)
        {
            foreach (var city in _citiesRepository.Cities)
            {
                if ((city.X + 60 >= x && city.X - 60 <= x) && (city.Y + 60 >= y && city.Y - 60 <= y))
                {
                    return false;
                }
            }
            //Paste Bob X,Y,B1
            //Set Zone 70+I,X-20,Y-20 To X+30,Y+30
            //// OR:
            // If Zone(X,Y)=0 and Zone(X+8,Y+8)=0 and Zone(X+4,Y+4)=0
            return true;
        }

        private List<Building> GenerateBuildings()
        {
            var buildings = new List<Building>();
            var x = 50;
            var y = 50;

            const int NumInitialBuildingsPerCity = 8; // For J=2 To 9
            for (var i = 0; i < NumInitialBuildingsPerCity; i++)
            {
                x += GlobalUtils.Rand(50);
                if (x > 580)
                {
                    x = 50;
                    y += 130 + GlobalUtils.Rand(30);
                }

                var building = GenerateBuilding(x, y);
                buildings.Add(building);

                x += building.Type.Width;
            }

            return buildings;
        }

        private Building GenerateBuilding(int x, int y)
        {
            var building = new Building();
            var type = _definitionsRepository.Buildings[GlobalUtils.Rand(_definitionsRepository.Buildings.Count-1)];
            building.Type = type;

            building.X = x;
            building.Y = y;

            return building;
        }
    }

}