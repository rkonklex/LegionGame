using Gui.Elements;
using Gui.Services;
using Microsoft.Xna.Framework.Graphics;

namespace Legion.Views.Map.Layers
{
    public class MapBackgroundLayer : MapLayer
    {
        private Texture2D _background;

        public MapBackgroundLayer(IGuiServices guiServices) : base(guiServices) { }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _background = GuiServices.ImagesStore.GetImage("map");
        }

        protected override void OnDraw()
        {
            base.OnDraw();
            GuiServices.BasicDrawer.DrawImage(_background, 0, 0);
        }
    }
}