using System;
using System.Collections.Generic;
using System.Drawing;
using Gui.Services;
using Legion.Utils;
using Legion.Model.Types;

namespace Legion.Views.Terrain
{
    public class TerrainGenerator
    {
        private readonly IImagesStore _imagesStore;

        public TerrainGenerator(IImagesStore imagesStore)
        {
            _imagesStore = imagesStore;
        }

        internal List<TerrainPart> Generate(Scenery scenery)
        {
            var images = scenery.Type switch
            {
                SceneryType.Forest => _imagesStore.GetImages("scene.forest"),
                SceneryType.Steppe => _imagesStore.GetImages("scene.steppe"),
                SceneryType.Desert => _imagesStore.GetImages("scene.desert"),
                SceneryType.Rocks => _imagesStore.GetImages("scene.rocks"),
                SceneryType.Snow => _imagesStore.GetImages("scene.snow"),
                SceneryType.Swamp => _imagesStore.GetImages("scene.swamp"),
                SceneryType.Cave => _imagesStore.GetImages("scene.cave"),
                SceneryType.Tomb => _imagesStore.GetImages("scene.tomb"),
                SceneryType.ChaosLordHome => _imagesStore.GetImages("scene.chaoslordhome"),
                _ => throw new ArgumentOutOfRangeException(nameof(scenery), scenery.Type, "Unknown scenery type"),
            };

            var parts = new List<TerrainPart>();

            var tile = images[0];
            var numTilesX = (640 + tile.Width - 1) / tile.Width;
            var numTilesY = (512 + tile.Height - 1) / tile.Height;
            for (var y = 0; y < numTilesY; y++)
            {
                for (var x = 0; x < numTilesX; x++)
                {
                    //Paste Bob X*50,Y*50,BIBY+1
                    parts.Add(new TerrainPart(tile, x * tile.Width, y * tile.Height));
                }
            }

            foreach (var item in scenery.Decorations)
            {
                // TODO: support Hrev
                parts.Add(new TerrainPart(images[item.Bob], item.X, item.Y));
            }

            return parts;
        }
    }
}