﻿using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_JSON;

namespace TheIdleScrolls_Storage
{
    public static class EntityJsonConversion
    {
        public static Entity EntityFromJson(JsonObject json)
        {
            var lookup = json.ToDictionary(x => x.Key, x => x.Value);
            Entity entity = new();

            var compTypes = new List<Type>()
            {
                typeof(AbilitiesComponent),
                typeof(AchievementsComponent),
                typeof(AttackComponent),
                typeof(BountyHunterComponent),
                typeof(CoinPurseComponent),
                typeof(CraftingBenchComponent),
                typeof(EquipmentComponent),
                typeof(InventoryComponent),
                typeof(ItemComponent),
                typeof(LevelComponent),
                typeof(LifePoolComponent),
                typeof(LocationComponent),
                typeof(NameComponent),
                typeof(PerksComponent),
                typeof(PlayerComponent),
                typeof(PlayerProgressComponent),
                typeof(ROMComponent),
                typeof(QuestProgressComponent),
                typeof(TitleBearerComponent),
                typeof(TravellerComponent),
                typeof(WeaponComponent),
                typeof(XpGainerComponent),
            };
            Dictionary<string, Type> components = compTypes
                .ToDictionary(x => x.Name.Replace("Component", ""), x => x);

            foreach (var kv in lookup)
            {
                IComponent component;
                try
                {
                    if (components.ContainsKey(kv.Key))
                    {
                        component = (IComponent)Activator.CreateInstance(components[kv.Key])!;
                    }
                    else
                    {
                        throw new Exception($"Unknown component type: {kv.Key}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                ComponentFromJson.SetFromJson(component as dynamic, kv.Value);
                entity.AddComponent(component as dynamic);
            }
            return entity;
        }

        public static JsonObject ToJson(this Entity entity)
        {
            JsonObject json = new JsonObject();
            foreach (var component in entity.Components)
            {
                JsonObject? jsonComp = JsonFromComponent.ToJson(component as dynamic);
                var key = (component as dynamic).GetType().Name.Replace("Component", "");
                if (key != "" && jsonComp != null)
                    json.Add(key, jsonComp);
            }
            return json;
        }
    }
}
