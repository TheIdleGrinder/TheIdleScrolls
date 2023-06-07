using MiniECS;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Storage;
using TheIdleScrolls_Storage;

namespace TheIdleScrolls_Web.CoreWrapper
{
    public class CoreWrapperModel : IApplicationModel
    {
        DataAccessHandler dataHandler;
        GameRunner gameRunner;

        public CoreWrapperModel()
        {
            dataHandler = new DataAccessHandler(new EntityJsonConverter(new ItemFactory()), new BasicFileStorageHandler());
            gameRunner = new GameRunner(dataHandler);
        }

        public HashSet<IMessage.PriorityLevel> GetRelevantMessagePriorties()
        {
            return new() { IMessage.PriorityLevel.High, IMessage.PriorityLevel.VeryHigh };
        }

        public List<string> GetSavedCharacters()
        {
            return dataHandler.ListStoredEntities();
        }
    }
}
