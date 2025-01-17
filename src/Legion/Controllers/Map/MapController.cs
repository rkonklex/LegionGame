using System.Collections.Generic;
using AwaitableCoroutine;
using Legion.Model;
using Legion.Model.Repositories;
using Legion.Model.Types;

namespace Legion.Controllers.Map
{
    public class MapController : IMapController
    {
        private readonly ICitiesRepository _citiesRepository;
        private readonly IArmiesRepository _armiesRepository;
        private readonly ICitiesTurnProcessor _citiesTurnProcessor;
        private readonly IArmiesTurnProcessor _armiesTurnProcessor;
        private readonly IWorldTurnProcessor _worldTurnProcessor;

        public MapController(
            ICitiesRepository citiesRepository,
            IArmiesRepository armiesRepository,
            ICitiesTurnProcessor citiesTurnProcessor,
            IArmiesTurnProcessor armiesTurnProcessor,
            IWorldTurnProcessor worldTurnProcessor)
        {
            _citiesRepository = citiesRepository;
            _armiesRepository = armiesRepository;
            _citiesTurnProcessor = citiesTurnProcessor;
            _armiesTurnProcessor = armiesTurnProcessor;
            _worldTurnProcessor = worldTurnProcessor;
        }

        public List<City> Cities => _citiesRepository.Cities;

        public List<Army> Armies => _armiesRepository.Armies;

        public bool IsProcessingTurn => _isProcessingTurn;

        private bool _isProcessingTurn;
        public async Coroutine StartNextTurn()
        {
            try
            {
                _isProcessingTurn = true;
                _worldTurnProcessor.NextTurn();
                await Coroutine.Yield();
                _citiesTurnProcessor.NextTurn();
                await Coroutine.Yield();
                await _armiesTurnProcessor.NextTurn();
            }
            finally
            {
                _isProcessingTurn = false;
            }
        }
    }
}