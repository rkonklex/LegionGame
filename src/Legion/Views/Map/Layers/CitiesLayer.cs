using Gui.Services;
using Legion.Controllers.Map;
using Legion.Views.Map.Controls;

namespace Legion.Views.Map.Layers
{
    public class CitiesLayer : MapLayer
    {
        private readonly IMapController _mapController;
        private readonly IMapCityGuiFactory _cityGuiFactory;

        public CitiesLayer(
            IGuiServices guiServices,
            IMapController mapController,
            IMapCityGuiFactory cityGuiFactory)
            : base(guiServices)
        {
            _mapController = mapController;
            _cityGuiFactory = cityGuiFactory;
        }

        public override void OnShow()
        {
            base.OnShow();
            
            ClearElements();

            foreach (var city in _mapController.Cities)
            {
                var element = new CityElement(GuiServices, city);
                element.Clicked += args =>
                {
                    var cityWindow = _cityGuiFactory.CreateCityWindow(city);
                    cityWindow.Open(Parent);
                    args.Handled = true;
                };
                AddElement(element);
            }
        }
    }
}