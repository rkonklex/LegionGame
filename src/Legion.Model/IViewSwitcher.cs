namespace Legion.Model
{
    public interface IViewSwitcher
    {
        // switching between Views
        void OpenMenu();
        void OpenMap();
        void OpenTerrain(TerrainActionContext context);
    }
}