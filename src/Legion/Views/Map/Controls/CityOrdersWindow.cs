using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gui.Services;
using Legion.Localization;

namespace Legion.Views.Map.Controls
{
    public class CityOrdersWindow : ButtonsListWindow
    {
        public CityOrdersWindow(IGuiServices guiServices, ITexts texts) : base(guiServices)
        {
            //TODO: provide translations for all items here
            ButtonNames = new Dictionary<string, Action<HandledEventArgs>>
            {
                {
                    texts.Get("cityMenu.taxes"), args =>
                    {
                        TaxesClicked?.Invoke(args);
                        Close();
                    }
                },
                {
                    texts.Get("cityMenu.newLegion"), args =>
                    {
                        NewLegionClicked?.Invoke(args);
                        Close();
                    }
                },
                {
                    texts.Get("cityMenu.newBuildings"), args =>
                    {
                        BuildClicked?.Invoke(args);
                        Close();
                    }
                },
                {
                    texts.Get("cityMenu.buildWalls"), args =>
                    {
                        WallsBuildClicked?.Invoke(args);
                        Close();
                    }
                },
                {
                    texts.Get("cityMenu.exit"), args =>
                    {
                        ExitClicked?.Invoke(args);
                        Close();
                    }
                }
            };
        }

        public event Action<HandledEventArgs> TaxesClicked;

        public event Action<HandledEventArgs> NewLegionClicked;

        public event Action<HandledEventArgs> BuildClicked;

        public event Action<HandledEventArgs> WallsBuildClicked;

        public event Action<HandledEventArgs> ExitClicked;
    }
}