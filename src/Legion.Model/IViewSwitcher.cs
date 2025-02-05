using AwaitableCoroutine;

namespace Legion.Model
{
    public interface IViewSwitcher
    {
        // switching between Views
        void OpenMenu();
        void OpenMap();
        void OpenTerrain(TerrainActionContext context);
    }

    public static class ViewSwitcherExtensions
    {
        public static Coroutine OpenTerrainAsync(this IViewSwitcher viewSwitcher, TerrainActionContext context)
        {
            viewSwitcher.OpenTerrain(context);
            return context.ActionFinished;
        }
    }
}