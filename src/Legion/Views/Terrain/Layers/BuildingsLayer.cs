using System.Collections.Generic;
using Gui.Elements;
using Gui.Services;
using Legion.Model;

namespace Legion.Views.Terrain.Layers
{
    public class BuildingsLayer : Layer
    {
        public BuildingsLayer(IGuiServices guiServices) : base(guiServices)
        {
        }

        private readonly List<TerrainPart> _terrainParts = new();

        public override void OnShow()
        {
            _terrainParts.Clear();
            var context = Parent.Context as TerrainActionContext;
            foreach (var building in context.Scenery.Buildings)
            {
                var img = GuiServices.ImagesStore.GetImageByRealName(building.Type.Img);
                _terrainParts.Add(new TerrainPart(img, building.X, building.Y));
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();
            foreach (var part in _terrainParts)
            {
                GuiServices.BasicDrawer.DrawImage(part.Image, part.X, part.Y);
            }
        }
    }
}