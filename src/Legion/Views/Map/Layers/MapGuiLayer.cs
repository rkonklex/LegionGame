using System.ComponentModel;
using AwaitableCoroutine;
using Gui.Services;
using Legion.Controllers.Map;
using Legion.Localization;
using Legion.Model;
using Legion.Views.Common;
using Legion.Views.Common.Controls;
using Legion.Views.Map.Controls;

namespace Legion.Views.Map.Layers
{
    public class MapGuiLayer : MapLayer
    {
        private readonly IMapController _mapController;
        private readonly ITexts _texts;
        private readonly IPlayersRepository _playersRepository;
        private readonly ILegionInfo _legionInfo;
        private readonly ICommonGuiFactory _commonGuiFactory;
        private readonly ICoroutineRunner _coroutineRunner;
        private MapMenu _mapMenu;

        public MapGuiLayer(
            IGuiServices guiServices,
            IMapController mapController,
            ITexts texts,
            IPlayersRepository playersRepository,
            ILegionInfo legionInfo,
            ICommonGuiFactory commonGuiFactory,
            ICoroutineRunner coroutineRunner) : base(guiServices)
        {
            _mapController = mapController;
            _texts = texts;
            _playersRepository = playersRepository;
            _legionInfo = legionInfo;
            _commonGuiFactory = commonGuiFactory;
            _coroutineRunner = coroutineRunner;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _mapMenu = new MapMenu(GuiServices);
            _mapMenu.StartClicked += OnStartClicked;
            _mapMenu.OptionsClicked += OnOptionsClicked;

            AddElement(_mapMenu);
        }

        private void OnOptionsClicked(HandledEventArgs args)
        {
            args.Handled = true;
            var window = new GameOptionsWindow(GuiServices, _texts, _playersRepository, _legionInfo);
            window.LoadGameClicked += _args =>
            {
                var loadGameWindow = _commonGuiFactory.CreateLoadGameWindow(null);
                loadGameWindow.Open(window.Parent);
            };
            window.StatisticsClicked += _args =>
            {
                var statsWindow = new GameStatisticsWindow(GuiServices, _texts, _legionInfo, _mapController, _playersRepository);
                statsWindow.ChartsClicked += __args =>
                {
                    var gameChartsWindow = new GameChartsWindow(GuiServices, _texts, _playersRepository);
                    gameChartsWindow.Open(statsWindow.Parent);
                };
                statsWindow.Open(window.Parent);
            };
            window.Open(Parent);
        }

        private Coroutine _turnTask;
        private void OnStartClicked(HandledEventArgs args)
        {
            args.Handled = true;
            if (_turnTask == null || _turnTask.IsCompleted)
            {
                _turnTask = _coroutineRunner.Create(_mapController.StartNextTurn);
            }
        }
    }
}