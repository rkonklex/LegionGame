using Legion.Model.Repositories;
using Legion.Model.Types;
using Legion.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Legion.Model
{
    public class WorldTurnProcessor : IWorldTurnProcessor
    {
        private readonly ILegionInfo _legionInfo;
        private readonly IArmiesRepository _armiesRepository;
        private readonly ICitiesRepository _citiesRepository;
        private readonly IPlayersRepository _playersRepository;

        public WorldTurnProcessor(ILegionInfo legionInfo,
            IArmiesRepository armiesRepository,
            ICitiesRepository citiesRepository,
            IPlayersRepository playersRepository)
        {
            _legionInfo = legionInfo;
            _armiesRepository = armiesRepository;
            _citiesRepository = citiesRepository;
            _playersRepository = playersRepository;
        }

        public void NextTurn()
        {
            _legionInfo.CurrentDay++;

            UpdateWorldPower();
            UpdateWars();
        }

        private void UpdateWars()
        {
            var userPlayerAndRivals = _playersRepository.Players.Where(p => !p.IsChaosControlled);
            foreach (var player in _playersRepository.Players)
            {
                player.DecreaseAllWars(1);
            }

            var averagePower = userPlayerAndRivals.Sum(p => p.Power) / userPlayerAndRivals.Count();
            var tooMuchPower = averagePower + (40 * averagePower) / 100;

            var leadPlayer = userPlayerAndRivals.MaxBy(p => p.Power);
            if (leadPlayer.Power > tooMuchPower)
            {
                foreach (var rival in userPlayerAndRivals)
                {
                    if (rival != leadPlayer && !rival.IsUserControlled)
                    {
                        rival.UpdateWar(leadPlayer, GlobalUtils.Rand(15) + 10);
                    }
                }
            }
        }

        private void UpdateWorldPower()
        {
            var userPlayerAndRivals = _playersRepository.Players.Where(p => !p.IsChaosControlled);
            foreach (var player in userPlayerAndRivals)
            {
                UpdatePlayerPower(player);
            }

            var newPower = 7 + _playersRepository.UserPlayer.Power / 900;
            if (newPower > 99) newPower = 99;
            _legionInfo.Power = newPower;
        }

        private void UpdatePlayerPower(Player player)
        {
            var armiesPower = 0;
            foreach (var army in _armiesRepository.Armies)
            {
                if (army.Owner == player && !army.IsKilled)
                {
                    armiesPower += army.Strength;
                }
            }

            var citiesPower = 0;
            foreach (var city in _citiesRepository.Cities)
            {
                if (city.Owner == player)
                {
                    citiesPower += city.Population * 2;
                }
            }

            var dayPower = _legionInfo.CurrentDay * 20;
            var moneyPower = player.Money / 10;
            player.Power = armiesPower + citiesPower + dayPower + moneyPower;
        }
    }
}