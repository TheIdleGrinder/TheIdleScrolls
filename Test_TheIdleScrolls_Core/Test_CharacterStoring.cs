using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;
using Test_TheIdleScrolls_Core.Utility;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Storage;
using MiniECS;
using TheIdleScrolls_Core.Components;

namespace Test_TheIdleScrolls_Core
{
    public class Test_DictionaryStorageHandler
    {
        [Test]
        public void Constructing_default_works()
        {
            var handler = new DictionaryStorageHandler();
            Assert.That(handler, Is.Not.Null);
        }

        [Test]
        public void CanStoreAndRetrieveData()
        {
            var handler = new DictionaryStorageHandler();
            handler.StoreData("A", "B");
            var result = handler.LoadData("A").Result;
            Assert.That(result, Is.EqualTo("B"));
        }

        [Test]
        public void CanDeleteData()
        {
            var handler = new DictionaryStorageHandler();
            handler.StoreData("A", "B");
            handler.DeleteData("A");
            var result = handler.LoadData("A").Result;
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void CanGetKeys()
        {
            var handler = new DictionaryStorageHandler();
            handler.StoreData("A", "B");
            handler.StoreData("C", "D");
            var keys = handler.GetKeys().Result;
            Assert.That(keys, Has.Count.EqualTo(2));
            Assert.That(keys, Contains.Item("A"));
            Assert.That(keys, Contains.Item("C"));
        }
    }

    public class Test_DataAccessHandler
    {
        [Test]
        public void Constructing_default_works()
        {
            var handler = new DataAccessHandler(new EntityJsonConverter(), new DictionaryStorageHandler());
            Assert.That(handler, Is.Not.Null);
        }

        [Test]
        public void CanStoreAndRetrieveEntity()
        {
            const string name = "B";
            var handler = new DataAccessHandler(new EntityJsonConverter(), new DictionaryStorageHandler());
            var entity = new Entity();
            entity.AddComponent(new NameComponent(name));
            handler.StoreEntity(entity).Wait();
                        
            var result = handler.LoadEntity(name).Result;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Components, Has.Count.EqualTo(1));
            Assert.That(result.HasComponent<NameComponent>());
            Assert.That(result.GetComponent<NameComponent>()!.Name, Is.EqualTo(name));
        }

    }
}
