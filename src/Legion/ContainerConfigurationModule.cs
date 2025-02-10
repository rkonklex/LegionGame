using Autofac;
using AwaitableCoroutine;
using Gui.Services;
using Legion.Archive;
using Legion.Controllers.Map;
using Legion.Controllers.Terrain;
using Legion.Localization;
using Legion.Model;
using Legion.Model.Helpers;
using Legion.Model.Repositories;
using Legion.Views;
using Legion.Views.Common;
using Legion.Views.Map;
using Legion.Views.Map.Layers;
using Legion.Views.Menu;
using Legion.Views.Menu.Layers;
using Legion.Views.Terrain;
using Legion.Views.Terrain.Layers;

namespace Legion
{
    public class ContainerConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LegionGame>().AsSelf().As<IGuiServices>().SingleInstance();
            builder.RegisterType<CoroutineRunner>().As<ICoroutineRunner>().SingleInstance();

            //Model
            builder.RegisterType<LegionConfig>().As<ILegionConfig>().SingleInstance();
            builder.RegisterType<LegionInfo>().As<ILegionInfo>().SingleInstance();
            builder.RegisterType<LanguageProvider>().As<ILanguageProvider>().SingleInstance();
            builder.RegisterType<Texts>().As<ITexts>().SingleInstance();

            builder.RegisterType<DefinitionsRepository>().As<IDefinitionsRepository>().SingleInstance();
            builder.RegisterType<CharacterDefinitionsRepository>().As<ICharacterDefinitionsRepository>().SingleInstance();
            builder.RegisterType<AdventuresRepository>().As<IAdventuresRepository>().SingleInstance();
            builder.RegisterType<ArmiesRepository>().As<IArmiesRepository>().SingleInstance();
            builder.RegisterType<CharactersRepository>().As<ICharactersRepository>().SingleInstance();
            builder.RegisterType<CitiesRepository>().As<ICitiesRepository>().SingleInstance();
            builder.RegisterType<PlayersRepository>().As<IPlayersRepository>().SingleInstance();

            builder.RegisterType<InitialDataGenerator>().As<IInitialDataGenerator>().SingleInstance();
            builder.RegisterType<GameArchive>().As<IGameArchive>().SingleInstance();
            builder.RegisterType<AmigaBinaryReader>().As<IBinaryReader>().SingleInstance();

            builder.RegisterType<CitiesHelper>().As<ICitiesHelper>().SingleInstance();
            builder.RegisterType<ArmiesHelper>().As<IArmiesHelper>().SingleInstance();
            builder.RegisterType<TerrainHelper>().As<ITerrainHelper>().SingleInstance();

            //Controllers
            builder.RegisterType<MapController>().As<IMapController>().SingleInstance();
            builder.RegisterType<TerrainController>().As<ITerrainController>().SingleInstance();

            builder.RegisterType<CityIncidents>().As<ICityIncidents>().SingleInstance();
            builder.RegisterType<BattleManager>().As<IBattleManager>().SingleInstance();
            builder.RegisterType<ArmyActivities>().As<IArmyActivities>().SingleInstance();

            builder.RegisterType<MapCityGuiFactory>().As<IMapCityGuiFactory>().SingleInstance();
            builder.RegisterType<MapArmyGuiFactory>().As<IMapArmyGuiFactory>().SingleInstance();
            builder.RegisterType<CommonMapGuiFactory>().As<ICommonMapGuiFactory>().SingleInstance();

            //Main Views
            builder.RegisterType<LegionViewsManager>().As<ILegionViewsManager>().As<IViewSwitcher>().SingleInstance();

            builder.RegisterType<MenuView>().As<MenuView>().SingleInstance();
            builder.RegisterType<MapView>().As<MapView>().SingleInstance();
            builder.RegisterType<TerrainView>().As<TerrainView>().SingleInstance();
            // Map Layers:
            builder.RegisterType<MapBackgroundLayer>().As<MapBackgroundLayer>().SingleInstance();
            builder.RegisterType<CitiesLayer>().As<CitiesLayer>().SingleInstance();
            builder.RegisterType<ArmiesLayer>().As<ArmiesLayer>().SingleInstance();
            builder.RegisterType<MapGuiLayer>().As<MapGuiLayer>().SingleInstance();
            builder.RegisterType<DrawingLayer>().As<DrawingLayer>().SingleInstance();
            // Menu Layers:
            builder.RegisterType<MenuLayer>().As<MenuLayer>().SingleInstance();

            // Terrain Layers:
            builder.RegisterType<TerrainLayer>().As<TerrainLayer>().SingleInstance();
            builder.RegisterType<BuildingsLayer>().As<BuildingsLayer>().SingleInstance();
            builder.RegisterType<CharactersLayer>().As<CharactersLayer>().SingleInstance();

            builder.RegisterType<MapMessagesService>().As<IMessagesService>().SingleInstance();
            builder.RegisterType<MapRouteDrawer>().As<IMapRouteDrawer>().SingleInstance();
            builder.RegisterType<CommonGuiFactory>().As<ICommonGuiFactory>().SingleInstance();

            builder.RegisterType<CitiesTurnProcessor>().As<ICitiesTurnProcessor>().SingleInstance();
            builder.RegisterType<ArmiesTurnProcessor>().As<IArmiesTurnProcessor>().SingleInstance();
            builder.RegisterType<WorldTurnProcessor>().As<IWorldTurnProcessor>().SingleInstance();
        }
    }
}
