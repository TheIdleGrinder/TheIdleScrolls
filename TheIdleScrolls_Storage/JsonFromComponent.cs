using System.Text.Json;
using System.Text.Json.Nodes;

using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;

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
            items.Select(item => itemFactory.GenerateItemCode(item))
                .ToList()
                .ForEach(j => result.Add(j));
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
                JsonObject jsonAbilty = (JsonObject)JsonObject.Parse(JsonSerializer.Serialize(ability))!;
                jsonAbilities.Add(jsonAbilty);
            }
            JsonObject json = new()
            {
                { "Abilities", jsonAbilities }
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

        public static JsonObject? ToJson(this WeaponComponent component)
        {
            JsonObject json = new()
            {
                { "Class", component.Class },
                { "Family", component.Family },
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
    }
}