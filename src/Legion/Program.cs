using Autofac;
using Legion.Model;
using System.IO;
using System.Reflection;

namespace Legion
{
    class Program
    {
        static void Main(string[] args)
        {
            // dotnet core sets current directory to the src folder by default
            // we need to change it to the folder where executable file location is
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ContainerConfigurationModule());

            using (var container = builder.Build())
            {
                var game = container.Resolve<LegionGame>();

                game.GameLoaded += () =>
                {
                    var initialDataGenerator = container.Resolve<IInitialDataGenerator>();
                    initialDataGenerator.Generate();

                    ////var archivePath = "/home/bartosz/Pobrane/dh0/legion/Legion/Archiwum/zapis 1";
                    //var archivePath = "/home/bartosz/Pobrane/_legion.lha/legion/Archiwum/Zapis 5";
                    //var gameArchive = container.Resolve<IGameArchive>();
                    //gameArchive.LoadGame(archivePath);
                };

                game.Run();
            }
        }
    }
}