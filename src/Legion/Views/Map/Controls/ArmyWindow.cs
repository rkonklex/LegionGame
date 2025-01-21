using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gui.Elements;
using Gui.Services;
using Legion.Localization;
using Legion.Model.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Legion.Views.Map.Controls
{
    public class ArmyWindow : Window
    {
        const int DefaultWidth = 150;
        const int DefaultHeight = 100;

        private readonly Army _army;
        private readonly ITexts _texts;

        private Panel _innerPanel;
        private Button _okButton;
        private Button _moreButton;
        private Label _nameLabel;
        private Label _countLabel;
        private Label _strengthLabel;
        private Label _foodLabel;
        private Label _speedLabel;
        private Label _actionLabel;
        private Label _infoLabel;
        private Image _image;

        public ArmyWindow(IGuiServices guiServices, ITexts texts, Army army) : base(guiServices)
        {
            _texts = texts;
            _army = army;
            CreateElements();
            UpdateBounds();
            UpdateArmyInfo();
        }

        public event Action<HandledEventArgs> OkClicked;

        public event Action<HandledEventArgs> MoreClicked;

        private void CreateElements()
        {
            _innerPanel = new Panel(GuiServices);

            _okButton = new BrownButton(GuiServices, _texts.Get("infoWindow.ok")) { Center = true };
            _okButton.Clicked += args =>
            {
                OkClicked?.Invoke(args);
                Close();
            };

            _moreButton = new BrownButton(GuiServices, "") { Center = true };
            _moreButton.Clicked += args =>
            {
                MoreClicked?.Invoke(args);
                Close();
            };

            _nameLabel = new Label(GuiServices);
            _countLabel = new Label(GuiServices);
            _strengthLabel = new Label(GuiServices);
            _foodLabel = new Label(GuiServices);
            _speedLabel = new Label(GuiServices);
            _actionLabel = new Label(GuiServices);
            _infoLabel = new Label(GuiServices);
            _image = new Image(GuiServices);

            AddElement(_innerPanel);
            AddElement(_okButton);
            AddElement(_moreButton);
            AddElement(_nameLabel);
            AddElement(_countLabel);
            AddElement(_strengthLabel);
            AddElement(_foodLabel);
            AddElement(_speedLabel);
            AddElement(_actionLabel);
            AddElement(_infoLabel);
            AddElement(_image);
        }

        private void UpdateArmyInfo()
        {
            _nameLabel.Text = _army.Name;

            var armyWindowImages = GuiServices.ImagesStore.GetImages("army.windowUsers");
            _image.Data = armyWindowImages[_army.Owner.Id - 1];

            var hasData = false;
            var infoText = "";
            var moreText = "";
            if (_army.IsUserControlled)
            {
                moreText = _texts.Get("infoWindow.commands");
                hasData = true;
            }
            else
            {
                infoText = _army.DaysToGetInfo switch
                {
                    > 28 and < 100 => _texts.Get("infoWindow.noInformation"),
                    > 1 => _texts.Get("infoWindow.informationsInXDays", _army.DaysToGetInfo),
                    1 => _texts.Get("infoWindow.informationsInOneDay"),
                    _ => "",
                };

                moreText = _texts.Get("infoWindow.inquiry");
                if (_army.DaysToGetInfo == 0 || _army.DaysToGetInfo == 100)
                {
                    moreText = _texts.Get("infoWindow.trace");
                    hasData = true;
                }
            }

            var countText = "";
            var foodText = "";
            var strengthText = "";
            var speedText = "";
            var actionText = "";
            if (hasData)
            {
                var count = _army.Characters.Count;
                countText = count switch
                {
                    1 => _texts.Get("armyInfo.oneWarrior"),
                    _ => _texts.Get("armyInfo.xWarriors", count),
                };

                int foodCount = _army.Food / _army.Characters.Count;
                foodText = foodCount switch
                {
                    > 1 => _texts.Get("armyInfo.foodForXDays", foodCount),
                    1 => _texts.Get("armyInfo.foodForOneDay"),
                    _ => _texts.Get("armyInfo.noMoreFood")
                };

                strengthText = _texts.Get("armyInfo.strength", _army.Strength);
                speedText = _texts.Get("armyInfo.speed", _army.Speed);

                actionText = _army.CurrentAction switch
                {
                    ArmyActions.Camping => _texts.Get("armyInfo.camping"),
                    /* TODO:
                     If TEREN>69
                        RO$=RO$+" w "+MIASTA$(TEREN-70)
                     End If 
                     */
                    ArmyActions.Move or ArmyActions.FastMove => _texts.Get("armyInfo.moving"),
                    ArmyActions.Attack => _texts.Get("armyInfo.attackingX", _army.Target.Name),
                    ArmyActions.Hunting => _texts.Get("armyInfo.hunting"),
                    _ => ""
                };
            }

            _moreButton.Text = moreText;
            _countLabel.Text = countText;
            _strengthLabel.Text = strengthText;
            _foodLabel.Text = foodText;
            _speedLabel.Text = speedText;
            _actionLabel.Text = actionText;
            _infoLabel.Text = infoText;
        }

        private void UpdateBounds()
        {
            var width = DefaultWidth;
            var height = DefaultHeight;
            var x = (GuiServices.GameBounds.Width / 2) - (width / 2);
            var y = (GuiServices.GameBounds.Height / 2) - (height / 2);
            Bounds = new Rectangle(x, y, width, height);

            _innerPanel.Bounds = new Rectangle(Bounds.X + 4, Bounds.Y + 4, 142, 74);
            _moreButton.Bounds = new Rectangle(Bounds.X + 4, Bounds.Y + 80, 52, 15);
            _okButton.Bounds = new Rectangle(Bounds.X + 106, Bounds.Y + 80, 40, 15);

            _nameLabel.Bounds = new Rectangle(Bounds.X + 50, Bounds.Y + 10, 10, 10);
            _countLabel.Bounds = new Rectangle(Bounds.X + 50, Bounds.Y + 20, 10, 10);
            _strengthLabel.Bounds = new Rectangle(Bounds.X + 50, Bounds.Y + 30, 10, 10);
            _foodLabel.Bounds = new Rectangle(Bounds.X + 50, Bounds.Y + 40, 10, 10);
            _speedLabel.Bounds = new Rectangle(Bounds.X + 50, Bounds.Y + 50, 10, 10);
            _actionLabel.Bounds = new Rectangle(Bounds.X + 12, Bounds.Y + 60, 10, 10);
            _infoLabel.Bounds = new Rectangle(Bounds.X + 25, Bounds.Y + 60, 10, 10);
            _image.Bounds = new Rectangle(Bounds.X + 8, Bounds.Y + 8, 1, 1);
        }
    }
}