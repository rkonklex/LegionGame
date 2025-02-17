using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gui.Elements;
using Gui.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Legion.Views.Map.Controls
{
    public class CityWindow : Window
    {
        protected const int DefaultWidth = 150;
        protected const int DefaultHeight = 100;

        protected Panel InnerPanel;
        protected Button OkButton;
        protected Button MoreButton;
        protected Label NameLabel;
        protected Label CountLabel;
        protected Label TaxLabel;
        protected Label MoraleLabel;
        protected Label InfoLabel;
        protected TextBlock BuildingsText;
        protected Image image;

        public CityWindow(IGuiServices guiServices) : base(guiServices)
        {
            CreateElements();
        }

        public string NameText
        {
            get => NameLabel.Text;
            set => NameLabel.Text = value;
        }

        public string CountText
        {
            get => CountLabel.Text;
            set => CountLabel.Text = value;
        }

        public string TaxText
        {
            get => TaxLabel.Text;
            set => TaxLabel.Text = value;
        }

        public string MoraleText
        {
            get => MoraleLabel.Text;
            set => MoraleLabel.Text = value;
        }

        public string InfoText
        {
            get => InfoLabel.Text;
            set
            {
                InfoLabel.Text = value;
                UpdateOkButtonBounds();
                UpdateMoreButtonVisibility();
            }
        }

        public Texture2D Image
        {
            get => image.Data;
            set => image.Data = value;
        }

        private List<string> _buildings;
        public List<string> Buildings
        {
            get => _buildings;
            set
            {
                _buildings = value;
                if (_buildings != null && _buildings.Count > 0)
                {
                    BuildingsText.Text = String.Join(' ', _buildings);
                }
                else
                {
                    BuildingsText.Text = "";
                }
            }
        }

        public string ButtonMoreText
        {
            get => MoreButton.Text;
            set => MoreButton.Text = value;
        }

        public string ButtonOkText
        {
            get => OkButton.Text;
            set => OkButton.Text = value;
        }

        public event Action<HandledEventArgs> OkClicked;

        public event Action<HandledEventArgs> MoreClicked;

        private void CreateElements()
        {
            InnerPanel = new Panel(GuiServices);

            OkButton = new BrownButton(GuiServices, "") { Center = true };
            OkButton.Clicked += args =>
            {
                OkClicked?.Invoke(args);
                Close();
            };

            MoreButton = new BrownButton(GuiServices, "") { Center = true };
            MoreButton.Clicked += args =>
            {
                MoreClicked?.Invoke(args);
                Close();
            };

            NameLabel = new Label(GuiServices);
            CountLabel = new Label(GuiServices);
            TaxLabel = new Label(GuiServices);
            MoraleLabel = new Label(GuiServices);
            InfoLabel = new Label(GuiServices);
            image = new Image(GuiServices);
            BuildingsText = new TextBlock(GuiServices) { TextColor = Color.Black };

            AddElement(InnerPanel);
            AddElement(OkButton);
            AddElement(MoreButton);
            AddElement(NameLabel);
            AddElement(CountLabel);
            AddElement(TaxLabel);
            AddElement(MoraleLabel);
            AddElement(InfoLabel);
            AddElement(image);
            AddElement(BuildingsText);

            UpdateBounds();
            UpdateMoreButtonVisibility();
        }

        private void UpdateBounds()
        {
            var width = DefaultWidth;
            var height = DefaultHeight;
            var x = (GuiServices.GameBounds.Width / 2) - (width / 2);
            var y = (GuiServices.GameBounds.Height / 2) - (height / 2);
            Bounds = new Rectangle(x, y, width, height);

            InnerPanel.Bounds = new Rectangle(Bounds.X + 4, Bounds.Y + 4, 142, 74);
            MoreButton.Bounds = new Rectangle(Bounds.X + 4, Bounds.Y + 80, 52, 15);
            NameLabel.Bounds = new Rectangle(Bounds.X + 50, Bounds.Y + 10, 10, 10);
            CountLabel.Bounds = new Rectangle(Bounds.X + 50, Bounds.Y + 20, 10, 10);
            TaxLabel.Bounds = new Rectangle(Bounds.X + 50, Bounds.Y + 30, 10, 10);
            MoraleLabel.Bounds = new Rectangle(Bounds.X + 50, Bounds.Y + 40, 10, 10);
            InfoLabel.Bounds = new Rectangle(Bounds.X + 50, Bounds.Y + 55, 10, 10);
            image.Bounds = new Rectangle(Bounds.X + 8, Bounds.Y + 8, 1, 1);
            BuildingsText.Bounds = new Rectangle(Bounds.X + 8, Bounds.Y + 52, 134, 20);

            UpdateOkButtonBounds();
        }

        private void UpdateOkButtonBounds()
        {
            var okX = !string.IsNullOrEmpty(InfoText) ? 55 : 106;
            OkButton.Bounds = new Rectangle(Bounds.X + okX, Bounds.Y + 80, 40, 15);
        }

        private void UpdateMoreButtonVisibility()
        {
            MoreButton.IsVisible = string.IsNullOrEmpty(InfoText);
        }
    }
}