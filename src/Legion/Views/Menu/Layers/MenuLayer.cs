using System.ComponentModel;
using System.IO;
using Gui.Elements;
using Gui.Input;
using Gui.Services;
using Legion.Archive;
using Legion.Localization;
using Legion.Model;
using Legion.Views.Common;
using Legion.Views.Common.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Legion.Views.Menu.Layers
{
    public class MenuLayer : Layer
    {
        private const int TopBoundary = 235;
        private const int BottomBoundary = 280;
        private const int SwordLeft = 80;
        private const int SwordTop = 190;
        private const int SwordMiddle = TopBoundary;
        private const int SwordBottom = 275;

        private LoadGameWindow _loadGameWindow;

        private Texture2D _background;
        private Texture2D _sword;
        private readonly IViewSwitcher _viewSwitcher;
        private readonly ITexts _texts;
        private readonly ICommonGuiFactory _commonGuiFactory;

        public MenuLayer(IGuiServices guiServices, 
            IViewSwitcher viewSwitcher,
            ITexts texts,
            ICommonGuiFactory commonGuiFactory) : base(guiServices)
        {
            _viewSwitcher = viewSwitcher;
            _texts = texts;
            _commonGuiFactory = commonGuiFactory;
            IsModal = true;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _background = GuiServices.ImagesStore.GetImage("mainMenu");
            _sword = GuiServices.ImagesStore.GetImage("mainMenuSword");
        }

        protected override void OnClick(HandledEventArgs args)
        {
            base.OnClick(args);

            if (_loadGameWindow == null)
            {
                args.Handled = true;
                var position = InputManager.GetMousePostion();
                if (position.Y < TopBoundary)
                {
                    _viewSwitcher.OpenMap();
                }
                else if (position.Y > BottomBoundary)
                {
                    // TODO: terminate game
                }
                else
                {
                    _loadGameWindow = _commonGuiFactory.CreateLoadGameWindow(args =>
                    {
                        RemoveElement(_loadGameWindow);
                        _loadGameWindow = null;
                        args.Handled = true;
                    });
                    AddElement(_loadGameWindow);
                }
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();
            GuiServices.BasicDrawer.DrawImage(_background, 0, 0);

            if (_loadGameWindow == null)
            {
                var swordY = SwordMiddle;
                var position = InputManager.GetMousePostion();
                if (position.Y < TopBoundary)
                {
                    swordY = SwordTop;
                }
                else if (position.Y > BottomBoundary)
                {
                    swordY = SwordBottom;
                }
                GuiServices.BasicDrawer.DrawImage(_sword, SwordLeft, swordY);
            }
        }
    }
}