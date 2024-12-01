using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;
using Test_TheIdleScrolls_Core.Utility;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Storage;
using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core;

namespace Test_TheIdleScrolls_Core
{
    public class Test_DataAccessHandler
    {
        [Test]
        public void Constructing_default_works()
        {
            var handler = new DataAccessHandler(
                new EntityJsonConverter(), 
                new DictionaryStorageHandler(), 
                new Base64ConversionDecorator<string>(
                    new InputToByteArrayConversionDecorator<byte[]>(
                        new NopDataEncryptor<byte[]>()
                    )
                )
            );
            Assert.That(handler, Is.Not.Null);
        }

        [Test]
        public void CanStoreAndRetrieveEntity()
        {
            var handler = new DataAccessHandler(new EntityJsonConverter(), new DictionaryStorageHandler());

            const string name = "TestChar";
            const int level = 15;
            var itemBlueprint = new ItemBlueprint(ItemFamilies.Dagger, 1, MaterialId.Metal1);
            List<ItemBlueprint> items = new() { itemBlueprint };
            var entity = MakeTestCharacter(name, level, items);

            handler.StoreEntity(entity).Wait();
                        
            var result = handler.LoadEntity(name).Result;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Components, Has.Count.EqualTo(entity.Components.Count));

            Assert.That(result.HasComponent<NameComponent>());
            Assert.That(result.GetComponent<NameComponent>()!.Name, Is.EqualTo(entity.GetName()));

            Assert.That(result.HasComponent<LevelComponent>());
            Assert.That(result.GetComponent<LevelComponent>()!.Level, Is.EqualTo(entity.GetLevel()));

            Assert.That(result.HasComponent<EquipmentComponent>());
            var equipmentComponent = result.GetComponent<EquipmentComponent>()!;
            Assert.That(equipmentComponent.GetItems(), Has.Count.EqualTo(entity.GetComponent<EquipmentComponent>()!.GetItems().Count));
            var item = equipmentComponent.GetItems().First();
            Assert.That(item.HasComponent<ItemComponent>());
            Assert.That(item.HasComponent<EquippableComponent>());
            Assert.That(item.GetComponent<ItemComponent>()!.Blueprint, Is.EqualTo(itemBlueprint));

            Assert.That(result.HasComponent<AbilitiesComponent>());
            var abilitiesComponent = result.GetComponent<AbilitiesComponent>()!;
            Assert.That(abilitiesComponent.GetAbilities(), Has.Count.EqualTo(entity.GetComponent<AbilitiesComponent>()!.GetAbilities().Count));
            var sbl = abilitiesComponent.GetAbility("SBL");
            Assert.That(sbl, Is.Not.Null);
            Assert.That(sbl!.Level, Is.EqualTo(12));
            var lar = abilitiesComponent.GetAbility("LAR");
            Assert.That(lar, Is.Not.Null);
            Assert.That(lar!.Level, Is.EqualTo(13));

            var meta = handler.GetCharacterMetaData(name).Result;
            Assert.That(meta, Is.Not.Null);
            Assert.That(meta.Name, Is.EqualTo(name));
            Assert.That(meta.Level, Is.EqualTo(level));
            Assert.That(meta.Class, Is.EqualTo("Adventurer"));
        }

        private static Entity MakeTestCharacter(string name, int level, List<ItemBlueprint> items)
        {
            var entity = new Entity();
            entity.AddComponent(new NameComponent(name));
            entity.AddComponent(new LevelComponent { Level = level });
            
            var equipment = new EquipmentComponent();
            foreach (var blueprint in items)
            {
                equipment.EquipItem(ItemFactory.MakeItem(blueprint)!);
            }            
            entity.AddComponent(equipment);

            var abilities = new AbilitiesComponent();
            Ability sbl = new("SBL")
            {
                Level = 12
            };
            abilities.AddAbility(sbl);
            Ability lar = new("LAR")
            {
                Level = 13
            };
            abilities.AddAbility(lar);
            entity.AddComponent(abilities);

            return entity;
        }
    }

    /// <summary>
    /// Tests the storage handler that is to be used by other tests for temporary storage.
    /// </summary>
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
}
