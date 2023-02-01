using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Legion.Model;
using Legion.Model.Repositories;
using Legion.Model.Types;
using Legion.Model.Types.Definitions;

namespace Legion.Archive
{
    public class GameArchive : IGameArchive
    {
        private readonly Func<byte[], IBinaryReader> _readerFactory;
        private readonly ILegionInfo _legionInfo;
        private readonly IArmiesRepository _armiesRepository;
        private readonly ICitiesRepository _citiesRepository;
        private readonly IPlayersRepository _playersRepository;
        private readonly IDefinitionsRepository _definitionsRepository;

        public GameArchive(
            Func<byte[], IBinaryReader> readerFactory,
            ILegionInfo legionInfo,
            IArmiesRepository armiesRepository,
            ICitiesRepository citiesRepository,
            IPlayersRepository playersRepository,
            IDefinitionsRepository definitionsRepository)
        {
            _readerFactory = readerFactory;
            _legionInfo = legionInfo;
            _armiesRepository = armiesRepository;
            _citiesRepository = citiesRepository;
            _playersRepository = playersRepository;
            _definitionsRepository = definitionsRepository;
        }

        public void LoadGame(string path)
        {
            var reader = _readerFactory(File.ReadAllBytes(path));

            var archiveName = reader.ReadCString(20);

            var players = new List<Player>();
            for (var id = 0; id <= 4; id++)
            {
                var player = new Player();
                player.Id = id;
                players.Add(player);
            }

            // Load armies and theirs characters data
            var armies = LoadArmies(reader, players);

            // TODO: Load conflicts info between players
            // 'wojna(5,5)
            reader.Skip(36);

            // Load players info
            LoadPlayers(reader, players);

            // Load armies and theirs characters names
            LoadArmiesNames(reader, armies);

            // Load player names
            LoadPlayerNames(reader, players);

            // TODO: Load preferences
            //'prefs(10) 
            reader.Skip(11);

            // Load cities info
            var cities = LoadCities(reader, players);

            // Load cities names
            LoadCitiesNames(reader, cities);

            // 44535 / 0xADF7

            _legionInfo.CurrentDay = reader.ReadInt16();
            _legionInfo.Power = reader.ReadInt16();

            // TODO: adventures:
            /*
            'przygody(3,10)
            For I=0 To 3
                For J=0 To 10
                    DAT=Deek(MEM)
                    PRZYGODY(I,J)=DAT
                    Add MEM,2
                Next J
            Next I
            'im_przygody$(3) 
            For I=0 To 3
                Gosub _READ_STRING
                IM_PRZYGODY$(I)=DAT$
            Next I
             */

            UpdateTargets(armies, cities);
            UpdateRepositories(armies, cities, players);
        }

        private void UpdateRepositories(List<Army> armies, List<City> cities, List<Player> players)
        {
            _armiesRepository.Armies.Clear();
            _citiesRepository.Cities.Clear();
            _playersRepository.Players.Clear();

            _armiesRepository.Armies.AddRange(armies);
            _citiesRepository.Cities.AddRange(cities);
            _playersRepository.Players.AddRange(players);
            _playersRepository.UserPlayer = players[1];
            _playersRepository.ChaosPlayer = players[players.Count - 1];
        }

        private List<Army> LoadArmies(IBinaryReader reader, List<Player> players)
        {
            var armies = new List<Army>();

            // Load Armies and theirs characters
            for (var id = 0; id <= 40; id++)
            {
                var armyData = reader.ReadInt16Array(ArmyInfo.ArrayLength);

                var army = new Army();
                army.Id = id;
                army.X = armyData[ArmyInfo.TX];
                army.Y = armyData[ArmyInfo.TY];
                army.Owner = players[armyData[ArmyInfo.TMAG]];
                army.DaysToGetInfo = armyData[ArmyInfo.TMAGMA];
                army.Food = armyData[ArmyInfo.TAMO];
                //NOTE: update target object later by providing correct reference to existing object (they have to be loaded first)
                army.Target = new MapPosition
                {
                    X = armyData[ArmyInfo.TCELX],
                    Y = armyData[ArmyInfo.TCELY]
                };
                army.CurrentAction = (ArmyActions) armyData[ArmyInfo.TTRYB];

                for (var ch = 1; ch <= 10; ch++)
                {
                    var characterData = reader.ReadInt16Array(ArmyInfo.ArrayLength);

                    if (characterData[ArmyInfo.TE] <= 0 || characterData[ArmyInfo.TEM] <= 0)
                    {
                        continue;
                    }

                    var character = new Character();
                    character.Id = ch;
                    character.EnergyMax = characterData[ArmyInfo.TEM];
                    character.X = characterData[ArmyInfo.TX];
                    character.Y = characterData[ArmyInfo.TY];
                    character.Strength = characterData[ArmyInfo.TSI];
                    character.Speed = characterData[ArmyInfo.TSZ];
                    character.SpeedMax = characterData[ArmyInfo.TAMO];
                    character.TargetX = characterData[ArmyInfo.TCELX];
                    character.TargetY = characterData[ArmyInfo.TCELY];
                    character.CurrentAction = (CharacterActionType)characterData[ArmyInfo.TTRYB];
                    character.Energy = characterData[ArmyInfo.TE];
                    character.Resistance = characterData[ArmyInfo.TP];
                    character.Magic = characterData[ArmyInfo.TMAG];
                    character.MagicMax = characterData[ArmyInfo.TMAGMA];
                    character.Experience = characterData[ArmyInfo.TDOSW];

                    character.Equipment = new CharacterEquipment();
                    character.Equipment.Head = GetItem(characterData[ArmyInfo.TGLOWA]);
                    character.Equipment.Torse = GetItem(characterData[ArmyInfo.TKLAT]);
                    character.Equipment.Feets = GetItem(characterData[ArmyInfo.TNOGI]);
                    character.Equipment.LeftHand = GetItem(characterData[ArmyInfo.TLEWA]);
                    character.Equipment.RightHand = GetItem(characterData[ArmyInfo.TPRAWA]);

                    for (var b = 0; b < 8; b++)
                    {
                        var itemType = characterData[ArmyInfo.TPLECAK + b];
                        character.Equipment.Backpack[b] = GetItem(itemType);
                    }

                    var characterType = characterData[ArmyInfo.TRASA];
                    character.Type = _definitionsRepository.Races[characterType];

                    army.Characters.Add(character);
                }

                if (army.Characters.Count > 0)
                {
                    armies.Add(army);
                }
            }

            return armies;
        }

        private Item GetItem(int type)
        {
            //--type;
            var def = _definitionsRepository.GetItemByOldIndex(type);
            if (def != null)
            {
                return new Item { Type = def };
            }
            return null;
        }

        private void LoadPlayers(IBinaryReader reader, List<Player> players)
        {
            for (var id = 0; id <= 4; id++)
            {
                var playerData = reader.ReadInt32Array(PlayerInfo.ArrayLength);

                var player = players[id];
                player.Money = playerData[PlayerInfo.P_MONEY];
                player.Power = playerData[PlayerInfo.P_POWER];
                player.Unknown = playerData[PlayerInfo.P_UNKNOWN];
            }
        }

        private void LoadArmiesNames(IBinaryReader reader, List<Army> armies)
        {
            for (var id = 0; id <= 40; id++)
            {
                Army army = null;
                var armyName = reader.ReadText();
                if (!string.IsNullOrEmpty(armyName))
                {
                    army = armies.FirstOrDefault(a => a.Id == id);
                    army.Name = armyName;
                }

                for (var chid = 1; chid <= 10; chid++)
                {
                    var characterName = reader.ReadText();
                    if (army != null && !string.IsNullOrEmpty(characterName))
                    {
                        var character = army.Characters.FirstOrDefault(ch => ch.Id == chid);
                        if (character != null)
                        {
                            character.Name = characterName;
                        }
                    }
                }
            }
        }

        private void LoadPlayerNames(IBinaryReader reader, List<Player> players)
        {
            for (var i = 0; i <= 4; i++)
            {
                var playerName = reader.ReadText();
                players[i].Name = playerName;
            }
        }

        private List<City> LoadCities(IBinaryReader reader, List<Player> players)
        {
            var cities = new List<City>();
            // Load cities info
            for (var id = 0; id <= 50; id++)
            {
                var cityData1 = reader.ReadInt16Array(CityInfo.ArrayLength);
                var cityData2 = reader.ReadInt16Array(CityInfo.ArrayLength);

                var city = new City();
                city.Id = id;
                city.WallType = cityData1[CityInfo.M_MUR];
                city.X = cityData1[CityInfo.M_X];
                city.Y = cityData1[CityInfo.M_Y];
                city.Population = cityData1[CityInfo.M_LUDZIE];
                city.Tax = cityData1[CityInfo.M_PODATEK];
                city.Owner = players[cityData1[CityInfo.M_CZYJE]];
                city.Morale = cityData1[CityInfo.M_MORALE];
                city.DaysToGetInfo = cityData2[CityInfo.M_Y];
                city.Food = cityData2[CityInfo.M_LUDZIE];
                city.DaysToSetNewRecruiters = cityData2[CityInfo.M_PODATEK];
                city.Craziness = cityData2[CityInfo.M_MORALE];

                for (var bid = 2; bid <= 20; bid++)
                {
                    var buildingData = reader.ReadInt16Array(CityInfo.ArrayLength);
                    //0, 1 are city info, rest are buildings
                    var building = new Building();
                    building.X = buildingData[CityInfo.M_X];
                    building.Y = buildingData[CityInfo.M_Y];
                    var buildingType = GetBuildingType(buildingData[CityInfo.M_LUDZIE]);
                    if (buildingType != null && building.X > 0 && building.Y > 0)
                    {
                        building.Type = buildingType;
                        city.Buildings.Add(building);
                    }
                }

                //TODO: price modificators for items:
                /*
                    For J=1 To 19
                        MIASTA(I,J,M_MUR)=Rnd(WAHANIA)
                    Next J
                */

                cities.Add(city);
            }
            return cities;
        }

        private BuildingDefinition GetBuildingType(int oid)
        {
            return _definitionsRepository.Buildings.FirstOrDefault(b => b.Oid == oid);
        }

        private void LoadCitiesNames(IBinaryReader reader, List<City> cities)
        {
            for (var id = 0; id <= 50; id++)
            {
                var name = reader.ReadText();
                if (!string.IsNullOrEmpty(name))
                {
                    var city = cities.FirstOrDefault(c => c.Id == id);
                    city.Name = name;
                }
            }
        }

        private void UpdateTargets(List<Army> armies, List<City> cities /*, List<Adventure> adventures */ )
        {
            foreach (var army in armies)
            {
                switch (army.CurrentAction)
                {
                    case ArmyActions.Camping:
                    case ArmyActions.Hunting:
                        army.Target = null;
                        break;
                    case ArmyActions.Move:
                    case ArmyActions.FastMove:
                        break;
                    case ArmyActions.Attack:
                        var targetId = army.Target.Y;
                        var isCityAttack = army.Target.X > 0;
                        if (isCityAttack)
                        {
                            army.Target = cities.FirstOrDefault(c => c.Id == targetId);
                        }
                        else
                        {
                            army.Target = armies.FirstOrDefault(a => a.Id == targetId);
                        }
                        break;
                }
            }
        }
    }
}