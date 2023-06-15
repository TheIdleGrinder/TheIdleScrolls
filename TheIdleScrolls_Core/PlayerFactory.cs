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
            player.AddComponent(new AbilitiesComponent());
            player.AddComponent(new AchievementsComponent());
            player.AddComponent(new CoinPurseComponent());
            player.AddComponent(new QuestProgressComponent());

            return player;
        }

        public static async Task<Entity> MakeOrLoadPlayer(string name, DataAccessHandler dataHandler)
        {
            var player = MakeNewPlayer(name);

            await dataHandler.LoadEntity(name, player);

            return player;
        }

        public static void SavePlayer(Entity player, DataAccessHandler dataHandler)
        {
            dataHandler.StoreEntity(player);
        }

        /// <summary>
        /// Finds a fitting character class name for the entity.
        /// CornerCut: This should probably not be part of the player factory, but it's only a single just-for-fun method...
        /// </summary>
        /// <param name="player">The character entity</param>
        /// <returns></returns>
        public static string GetCharacterClass(Entity character)
        {
            int level = character.GetComponent<LevelComponent>()?.Level ?? 0;
            var abiComp = character.GetComponent<AbilitiesComponent>();
            if (level == 0 || abiComp == null)
            {
                return String.Empty;
            }
            if (level < 20)
            {
                return "CLASS_DEFAULT".Localize();
            }
            List<string> weapons = new() { "AXE", "BLN", "LBL", "POL", "SBL" };
            List<string> armor = new() { "HAR", "LAR" };

            var BestAbility = (List<string> keys) =>
            {
                return keys
                .Select(k => abiComp.GetAbility(k))
                .Where(a => a != null && a.Level >= level * 2 / 3)
                .OrderByDescending(a => a!.Level)
                .FirstOrDefault()?.Key ?? "X";
            };

            string bestWeapon = BestAbility(weapons);
            string bestArmor = BestAbility(armor);
            return $"CLASS_{bestWeapon}_{bestArmor}";
        }
    }
}
