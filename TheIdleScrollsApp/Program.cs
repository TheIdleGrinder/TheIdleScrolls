using TheIdleScrolls_Core;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Storage;
using TheIdleScrolls_Storage;

namespace TheIdleScrollsApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string playerName = "Leeroy";
            if (args.Length >= 2 && args[1] != "")
                playerName = args[1];

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            //IEntityConverter converter = new BasicPlayerEntityConverter();
            IEntityConverter converter = new EntityJsonConverter(new ItemFactory());

            var dataHandler = new DataAccessHandler(converter, new BasicFileStorageHandler());
            var gameRunner = new GameRunner(dataHandler, false);

            Application.Run(new MainWindow(gameRunner, playerName));
        }
    }
}