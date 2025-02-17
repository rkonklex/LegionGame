using System;
using System.ComponentModel;
using Gui.Elements;
using Gui.Services;
using Legion.Localization;
using Legion.Model;
using Microsoft.Xna.Framework;

namespace Legion.Views.Map.Controls
{
    public class GameChartsWindow : Window
    {
        protected const int DefaultWidth = 160;
        protected const int DefaultHeight = 120;

        private readonly ITexts _texts;
        private readonly IPlayersRepository _playersRepository;

        protected Panel InnerPanel;
        protected Button OkButton;
        protected Button ChartsButton;

        public GameChartsWindow(
            IGuiServices guiServices, 
            ITexts texts,
            IPlayersRepository playersRepository) : base(guiServices)
        {
            _texts = texts;
            _playersRepository = playersRepository;
            CreateElements();
        }

        public event Action<HandledEventArgs> OkClicked;

        public event Action<HandledEventArgs> ChartsClicked;

        private void CreateElements()
        {
            var width = DefaultWidth;
            var height = DefaultHeight;
            var x = (GuiServices.GameBounds.Width / 2) - (width / 2);
            var y = (GuiServices.GameBounds.Height / 2) - (height / 2);
            Bounds = new Rectangle(x, y, width, height);

            InnerPanel = new Panel(GuiServices);
            InnerPanel.Bounds = new Rectangle(Bounds.X + 4, Bounds.Y + 4, 152, 92);
            AddElement(InnerPanel);

            OkButton = new BrownButton(GuiServices, _texts.Get("gameStatistics.ok")) { Center = true };
            OkButton.Bounds = new Rectangle(Bounds.X + 116, Bounds.Y + 100, 40, 15);
            OkButton.Clicked += args =>
            {
                OkClicked?.Invoke(args);
                Close();
            };

            ChartsButton = new BrownButton(GuiServices, _texts.Get("gameStatistics.charts")) { Center = true };
            ChartsButton.Bounds = new Rectangle(Bounds.X + 4, Bounds.Y + 100, 40, 15);
            ChartsButton.Clicked += args =>
            {
                ChartsClicked?.Invoke(args);
                Close();
            };
            
            for (int i = 1; i < _playersRepository.Players.Count; i++)
            {
                var player = _playersRepository.Players[i];

                var playerElement = new PlayerChartElement(GuiServices, player);
                playerElement.Bounds = new Rectangle(Bounds.X, Bounds.Y + 8 + (i-1) * 20, 10, 10);
                AddElement(playerElement);
            }
            
            AddElement(OkButton);
            AddElement(ChartsButton);
        }
    }
}