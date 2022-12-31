using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Storage;

namespace TheIdleScrolls_Core
{
    public class PlayerFactory
    {
        public static Entity MakeNewPlayer(string name)
        {
            var player = new Entity();
            player.AddComponent(new PlayerComponent());
            player.AddComponent(new NameComponent(name));
            player.AddComponent(new LevelComponent { Level = 1 });
            player.AddComponent(new AttackComponent { RawDamage = 2.0, Cooldown = new(1.0) });
            player.AddComponent(new DefenseComponent());
            player.AddComponent(new XpGainerComponent());
            player.AddComponent(new PlayerProgressComponent());

            AbilitiesComponent abilitiesComp = new();
            ItemFactory itemFactory = new();
            foreach (string key in ItemFactory.GetAllItemFamilyIds())
            {
                string? className = ItemFactory.GetItemFamilyName(key);
                if (className == null)
                {
                    Debug.WriteLine($"Failed to retrieve class name for {key}");
                    continue;
                }
                Ability ability = new Ability(key, className);
                ability.Level = 10;
                abilitiesComp.AddAbility(ability);
            }
            player.AddComponent(abilitiesComp);

            return player;
        }

        public static Entity MakeOrLoadPlayer(string name, DataAccessHandler dataHandler)
        {
            var player = MakeNewPlayer(name);

            dataHandler.LoadEntity(name, player);

            return player;
        }

        public static void SavePlayer(Entity player, DataAccessHandler dataHandler)
        {
            dataHandler.StoreEntity(player);
        }
    }
}
