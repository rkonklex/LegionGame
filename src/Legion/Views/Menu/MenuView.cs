using System.Collections.Generic;
using Gui.Elements;
using Gui.Services;
using Legion.Views.Menu.Layers;

namespace Legion.Views.Menu
{
    public class MenuView : View
    {
        public MenuView(IGuiServices guiServices,
            MenuLayer menuLayer) : base(guiServices)
        {
            AddLayer(menuLayer);
        }
    }
}