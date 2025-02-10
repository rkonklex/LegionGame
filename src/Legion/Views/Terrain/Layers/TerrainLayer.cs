using System.Collections.Generic;
using Gui.Elements;
using Gui.Services;
using Legion.Model;

namespace Legion.Views.Terrain.Layers
{
    public class TerrainLayer : Layer
    {
        private TerrainGenerator _terrainGenerator;
        private List<TerrainPart> _terrainParts;

        public TerrainLayer(IGuiServices guiServices) : base(guiServices) { }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _terrainGenerator = new TerrainGenerator(GuiServices.ImagesStore);
        }

        public override void OnShow()
        {
            var context = Parent.Context as TerrainActionContext;
            _terrainParts = _terrainGenerator.Generate(context.Scenery);
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