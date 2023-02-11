using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;

using MiniECS;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;
using IComponent = MiniECS.IComponent;

namespace TheIdleScrolls_Storage
{
    public static class JsonFromComponent
    {
        public static JsonObject? ToJson(this IComponent component)
        {
            string name = (component as dynamic).GetType().Name.Replace("Component", "");
            return new JsonObject();
        }

        static JsonArray JsonArrayFromItemList(List<Entity> items)
        {
            var itemFactory = new ItemFactory();
            var result = new JsonArray();

            foreach (var item in items)
            {
                string? code = item.GetItemCode();
                if (code == String.Empty)
                {
                    continue;
                }
                if (item.GetComponent<ItemReforgeableComponent>()?.Reforged ?? false)
                {
                    code += " #c";
                }
                result.Add(code);
            }
            return result;
        }

        public static JsonObject? ToJson(this EquipmentComponent component)
        {
            JsonObject json = new()
            {
                { "Items", JsonArrayFromItemList(component.GetItems()) }
            };
            return json;
        }

        public static JsonObject? ToJson(this InventoryComponent component)
        {
            JsonObject json = new()
            {
                { "Items", JsonArrayFromItemList(component.GetItems()) }
            };
            return json;
        }

        public static JsonObject? ToJson(this AbilitiesComponent component)
        {
            JsonArray jsonAbilities = new();
            foreach (var ability in component.GetAbilities())
            {
                string shortened = $"{ability.Key}/{ability.Level}/{ability.XP}";
                //JsonObject jsonAbilty = JsonFromSth(shortened);
                jsonAbilities.Add(shortened);
            }
            JsonObject json = new()
            {
                { "Abilities", jsonAbilities }
            };
            return json;
        }

        public static JsonObject? ToJson(this AchievementsComponent component)
        {
            JsonArray earned = new();
            component.Achievements.Where(a => a.Status == AchievementStatus.Awarded).ToList().ForEach(a => earned.Add(a.Id));
            component.EarnedAchievements.ToList().ForEach(a => earned.Add(a));
            JsonObject json = new()
            {
                { "Earned", earned }
            };
            return json;
        }

        public static JsonObject? ToJson(this LevelComponent component)
        {
            JsonObject json = new()
            {
                { "Level", component.Level }
            };
            return json;
        }

        public static JsonObject? ToJson(this LifePoolComponent component)
        {
            JsonObject json = new()
            {
                { "Maximum", component.Maximum },
                { "Current", component.Current }
            };
            return json;
        }

        public static JsonObject? ToJson(this NameComponent component)
        {
            JsonObject json = new()
            {
                { "Name", component.Name }
            };
            return json;
        }

        public static JsonObject? ToJson(this ItemComponent component)
        {
            JsonObject json = new()
            {
                { "Code", component.Code.Code }
            };
            return json;
        }

        public static JsonObject? ToJson(this WeaponComponent component)
        {
            JsonObject json = new()
            {
                { "Damage", component.Damage },
                { "Cooldown", component.Cooldown }
            };
            return json;
        }

        public static JsonObject? ToJson(this XpGainerComponent component)
        {
            JsonObject json = new()
            {
                { "Current", component.Current }
            };
            return json;
        }

        public static JsonObject? ToJson(this PlayerProgressComponent component)
        {
            JsonObject json = new()
            {
                { "Data", JsonFromSth(component.Data) }
            };
            return json;
        }

        public static JsonObject? ToJson(this CoinPurseComponent component)
        {
            JsonObject json = new()
            {
                { "Coins", component.Coins }
            };
            return json;
        }

        public static JsonObject JsonFromSth<T>(T thing)
        {
            return (JsonObject)JsonObject.Parse(JsonSerializer.Serialize(thing))!;
        }
    }
}