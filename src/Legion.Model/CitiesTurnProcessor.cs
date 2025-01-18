using System;
using System.Linq;
using AwaitableCoroutine;
using Legion.Model.Repositories;
using Legion.Model.Types;
using Legion.Utils;

namespace Legion.Model
{
    public class CitiesTurnProcessor : ICitiesTurnProcessor
    {
        private readonly ICitiesRepository _citiesRepository;
        private readonly IArmiesRepository _armiesRepository;
        private readonly ICityIncidents _cityIncidents;
        private readonly ILegionInfo _legionInfo;

        public CitiesTurnProcessor(ICitiesRepository citiesRepository,
            IArmiesRepository armiesRepository,
            ICityIncidents cityIncidents,
            ILegionInfo legionInfo)
        {
            _citiesRepository = citiesRepository;
            _armiesRepository = armiesRepository;
            _cityIncidents = cityIncidents;
            _legionInfo = legionInfo;
        }

        public bool IsProcessingTurn { get; private set; }

        public async Coroutine NextTurn()
        {
            if (IsProcessingTurn)
            {
                throw new InvalidOperationException("Turn is already in progress");
            }

            IsProcessingTurn = true;
            try
            {
                // Create a copy of the list to avoid modification during iteration
                var citiesSnapshot = _citiesRepository.Cities.ToList();

                foreach (var city in citiesSnapshot)
                {
                    await ProcessTurn(city);
                }
            }
            finally
            {
                IsProcessingTurn = false;
            }
        }

        private async Coroutine ProcessTurn(City city)
        {
            if (city.DaysToGetInfo < 25 && city.DaysToGetInfo > 0) city.DaysToGetInfo--;
            if (city.DaysToSetNewRecruiters > 0) city.DaysToSetNewRecruiters--;

            if (GlobalUtils.Rand(50) == 1 && city.Population > 800)
            {
                await _cityIncidents.Plague(city, GlobalUtils.Rand(2));
            }

            if (GlobalUtils.Rand(5) == 1)
            {
                //TODO: Add MIASTA(M,1,M_MORALE),Rnd(2)-1,0 To 25
                //' V = V + A
                //' V<BASE Then V = TOP
                //' V> TOP Then V = BASE
                var x = city.Craziness + GlobalUtils.Rand(2) - 1;
                if (x < 0) x = 25;
                if (x > 25) x = 0;
                city.Craziness = x;
            }

            ProcessTaxes(city);
            ProcessGranaries(city);
            await ProcessPopulationAsync(city);
            ProcessRecruiting(city);
        }

        private void ProcessTaxes(City city)
        {
            if (city.Owner != null)
            {
                city.Owner.Money += city.Tax * city.Population / 25;
            }
        }

        private void ProcessGranaries(City city)
        {
            // obsługa spichlerzy
            var granariesCount = city.Buildings.Count(b => b.Type.Name == "granary");
            if (granariesCount > 0)
            {
                //If SPI>0 : Add MIASTA(M,1,M_LUDZIE),LUDZIE/15,MIASTA(M,1,M_LUDZIE) To SPI*200 : End If 
                var newFood = city.Food + city.Population / 15;
                var maxFood = granariesCount * 200;
                if (newFood <= maxFood) city.Food = newFood;
            }
        }

        private async Coroutine ProcessPopulationAsync(City city)
        {
            if (city.Owner != null)
            {
                if (city.Owner.IsUserControlled)
                {
                    var population = city.Population + (city.Craziness - city.Tax);
                    var morale = city.Morale + (city.Craziness - city.Tax);

                    if (morale > 150) morale = 150;
                    if (population < 30) population = 30;
                    city.Population = population;
                    city.Morale = morale;

                    if (morale <= 0)
                    {
                        await _cityIncidents.Riot(city);
                    }
                }
                else
                {
                    city.Population += GlobalUtils.Rand(10) - 2;
                }
            }
        }

        private void ProcessRecruiting(City city)
        {
            // NOTE: some old game saves have cities which belongs to owner with id == zero
            if (city.Owner != null && !city.Owner.IsUserControlled && city.Owner.Id > 0)
            {
                if (city.Owner.Money > 10000 &&GlobalUtils.Rand(3) == 1 && city.DaysToSetNewRecruiters == 0)
                {
                    // TODO: set upper limit for player's legion count // For I=20 To 39
                    city.Owner.Money -= 10000;
                    city.DaysToSetNewRecruiters = 20 + GlobalUtils.Rand(10);
                    var army = _armiesRepository.CreateNpcArmy(city.Owner, 10, _legionInfo.Power);
                    army.X = city.X;
                    army.Y = city.Y;
                }
            }
        }
    }
}