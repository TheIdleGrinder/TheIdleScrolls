﻿using MiniECS;
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
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Crafting;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Resources;
using TheIdleScrolls_Core.Systems;
using TheIdleScrolls_Storage;

namespace TheIdleScrolls_JSON
{
    public static class ComponentFromJson
    {
        #pragma warning disable IDE0060
        public static bool SetFromJson(this MiniECS.IComponent component, JsonNode json)
        {
            return false;
        }
        #pragma warning restore IDE0060

        static List<Entity> ItemListFromJsonArray(JsonArray jsonItems)
        {
            List<Entity> items = new();
            foreach (var jsonItem in jsonItems)
            {
                if (jsonItem == null)
                    continue;

                string[] fields = jsonItem.GetValue<string>().Split(' ');
                string code = fields[0];
                string[] tags = fields[1..];

                Entity? item = ItemFactory.ExpandCode(code);
                if (item == null)
                {
                    Debug.WriteLine($"Unable to expand item code {jsonItem}");
                    continue;
                }

                foreach (string tag in tags)
                {
                    switch (tag.ToUpper())
                    {
                        case "#C":
                            {
                                var refineComp = item.GetComponent<ItemRefinableComponent>();
                                if (refineComp != null)
                                    refineComp.Refined = true; 
                                break;
                            }
                        default: 
                            continue;
                    }
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
                    if (fields.Length != 3)
                        throw new Exception($"Invalid number of fields in stored ability: {jsonAbility}");
                    string key = fields[0];
                    int level = Int32.Parse(fields[1]);
                    int xp = Int32.Parse(fields[2]);

                    var abilityDef = AbilityList.GetAbility(key) 
                                    ?? AbilityList.GetAbility("ABL_" + key); // Backwards compatibility
                    if (abilityDef == null)
                    {
                        Console.WriteLine($"Ability {key} not found in AbilityList");
                        continue; // Skip silently (ability might no longer be part of the game)
                    }

                    component.AddAbility(key);
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

        public static bool SetFromJson(this PlayerComponent component, JsonNode json)
        {
            try
            {
                var features = JsonSerializer.Deserialize<List<GameFeature>>(json["Features"]!)!.ToHashSet();
                if (features != null)
                   component.AvailableFeatures = features;
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
                component.Blueprint = ItemBlueprint.Parse(json["Code"]!.GetValue<string>());
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

        public static bool SetFromJson(this QuestProgressComponent component, JsonNode json)
        {
            try
            {
                //component.FinalFight.State = json["Finished"]!.GetValue<bool>()
                //    ? FinalFight.Status.Finished 
                //    : FinalFight.Status.NotStarted;
                component.Quests = JsonSerializer.Deserialize<Dictionary<QuestId, int>>(json["QuestStates"]!)!;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this LocationComponent component, JsonNode json)
        {
            try
            {
                component.CurrentLocation = Location.FromString(json["Location"]!.GetValue<string>());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this CraftingBenchComponent component, JsonNode json)
        {
			try
            {
				component.CraftingSlots = json["Slots"]!.GetValue<int>();
				var jsonCrafts = json["ActiveCrafts"]!.AsArray();
				foreach (var jsonCraft in jsonCrafts)
                {
                    string[] parts = jsonCraft!.ToString().Split('/');
                    if (parts.Length != 6)
						throw new Exception($"Invalid number of fields in stored craft: {jsonCraft}");
                    CraftingType type = (CraftingType)Int32.Parse(parts[0]);
					Entity item = ItemFactory.MakeItem(ItemBlueprint.Parse(parts[1])) 
                        ?? throw new Exception($"Unable to make item from code {parts[1]}");
                    double duration = Double.Parse(parts[2]);
                    double remaining = Double.Parse(parts[3]);
                    double roll = Double.Parse(parts[4]);
                    int cost = Int32.Parse(parts[5]);
					CraftingProcess process = new(type, item, duration, roll, cost);
                    process.Update(duration - remaining);
                    component.ActiveCrafts.Add(process);
				}
				return true;
			}
			catch (Exception)
            {
				return false;
			}
		}

        public static bool SetFromJson(this BountyHunterComponent component, JsonNode json)
        {
            try
            {
                component.HighestCollected = json["HighestCollected"]?.GetValue<int>() ?? 0;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this PerksComponent component, JsonNode json)
        {
            try
            {
                component.PerkLevels.Clear();
                var jsonPerks = json["PerkLevels"]!.AsArray();
                foreach (var jsonPerk in jsonPerks)
                {
                    string line = jsonPerk!.GetValue<string>();
                    int splitAt = line.LastIndexOf(':');
                    if (splitAt == -1)
                        throw new Exception($"Invalid perk level line: {line}");
                    string id = line[..splitAt];
                    int level = Int32.Parse(line[(splitAt + 1)..]);
                    component.PerkLevels[id] = level;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetFromJson(this TitleBearerComponent component, JsonNode json)
        {
            try
            {
                component.Titles = json["Titles"]!.AsArray()
                    .Select(j => j!.GetValue<string>())
                    .ToHashSet();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
