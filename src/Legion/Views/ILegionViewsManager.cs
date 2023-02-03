using Gui.Elements;
using Gui.Services;
using Legion.Model;

namespace Legion.Views
{
    public interface ILegionViewsManager : IViewsManager
    {
        void OpenMenu();
        void OpenMap();
        void OpenTerrain(TerrainActionContext context);
    }
}