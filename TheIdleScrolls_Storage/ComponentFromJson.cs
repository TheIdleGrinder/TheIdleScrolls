using MiniECS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Storage;

namespace TheIdleScrolls_JSON
{
    public static class ComponentFromJson
    {
        public static bool SetFromJson(this MiniECS.IComponent component, JsonNode json)
        {
            return false;
        }

        static List<Entity> ItemListFromJsonArray(JsonArray jsonItems)
        {
            var itemFactory = new ItemFactory();
            List<Entity> items = new();
            foreach (var jsonItem in jsonItems)
            {
                if (jsonItem == null)
                    continue;
                Entity? item = itemFactory.ExpandCode(jsonItem.GetValue<string>());
                if (item == null)
                {
                    Debug.WriteLine($"Unable to expand item code {jsonItem}");
                    continue;
                }
                items.Add(item);
            }
            return items;
        }

        public static bool SetFromJson(this EquipmentComponent component, JsonNode json)
        {
            try
            {
                var jsonItems = json["Items"]!.AsArray();
                var items = ItemListFromJsonArray(jsonItems);
                items.ForEach(item => component.EquipItem(item));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this InventoryComponent component, JsonNode json)
        {
            try
            {
                var jsonItems = json["Items"]!.AsArray();
                var items = ItemListFromJsonArray(jsonItems);
                items.ForEach(item => component.AddItem(item));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this AbilitiesComponent component, JsonNode json)
        {
            try
            {
                var jsonAbs = json["Abilities"]!.AsArray();
                foreach (var jsonAbility in jsonAbs)
                {
                    if (jsonAbility == null)
                        continue;
                    var fields = jsonAbility.ToString().Split('/');
                    if (fields.Count() != 3)
                        throw new Exception($"Invalid number of fields in stored ability: {jsonAbility}");
                    string key = fields[0];
                    int level = Int32.Parse(fields[1]);
                    int xp = Int32.Parse(fields[2]);

                    if (component.GetAbility(key) == null)
                        continue; // Skip silently (ability might no longer be part of the game)

                    component.UpdateAbility(key, level, xp);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this AchievementsComponent component, JsonNode json)
        {
            try
            {
                var earned = json["Earned"]!.AsArray();
                foreach (var e in earned)
                {
                    if (e == null)
                        continue;
                    component.EarnedAchievements.Add(e.ToString());
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this LevelComponent component, JsonNode json)
        {
            try
            {
                component.Level = json["Level"]!.GetValue<int>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this LifePoolComponent component, JsonNode json)
        {
            try
            {
                component.Current = json["Current"]!.GetValue<int>();
                component.Maximum = json["Maximum"]!.GetValue<int>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this NameComponent component, JsonNode json)
        {
            try
            {
                component.Name = json["Name"]!.GetValue<string>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this ItemComponent component, JsonNode json)
        {
            try
            {
                component.Code = new ItemIdentifier(json["Code"]!.GetValue<string>());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this WeaponComponent component, JsonNode json)
        {
            try
            {
                component.Damage = json["Damage"]!.GetValue<double>();
                component.Cooldown = json["Cooldown"]!.GetValue<double>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this XpGainerComponent component, JsonNode json)
        {
            try
            {
                component.Current = json["Current"]!.GetValue<int>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this PlayerProgressComponent component, JsonNode json)
        {
            try 
            {
                component.Data = JsonSerializer.Deserialize<ProgressData>(json["Data"]!)!;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this CoinPurseComponent component, JsonNode json)
        {
            try
            {
                component.Coins = json["Coins"]!.GetValue<int>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
