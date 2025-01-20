using Gui.Services;
using Legion.Controllers.Map;
using Legion.Views.Map.Controls;

namespace Legion.Views.Map.Layers
{
    public class CitiesLayer : MapLayer
    {
        private readonly IMapController _mapController;
        private readonly IMapCityGuiFactory _cityGuiFactory;
        private readonly ModalLayer _modalLayer;

        public CitiesLayer(
            IGuiServices guiServices,
            IMapController mapController,
            IMapCityGuiFactory cityGuiFactory,
            ModalLayer modalLayer)
            : base(guiServices)
        {
            _mapController = mapController;
            _cityGuiFactory = cityGuiFactory;
            _modalLayer = modalLayer;
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
                    _modalLayer.Window = cityWindow;
                    args.Handled = true;
                };
                AddElement(element);
            }
        }
    }
}