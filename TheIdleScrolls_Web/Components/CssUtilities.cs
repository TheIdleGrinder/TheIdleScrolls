using TheIdleScrolls_Core;

namespace TheIdleScrolls_Web.Components
{
    public class CssUtilities
    {
        public static string GetClassesForItem(ItemRepresentation? item)
        {
            string classes = "";
            if (item == null || item.Rarity < 0)
            {
                classes += " rarity-none";
            }
            else
            {
                classes += $" rarity-{item.Rarity}";
                if (item.Rarity > 0)
                    classes += " rare-item";
            }

            if (item?.Crafted ?? false)
                classes += " crafted-item";

            return classes;
        }

        public static string GetClassesForDungeon(DungeonRepresentation dungeon)
        {
            string classes = $"rarity-{dungeon.Rarity}";
            if (dungeon.Rarity > 0)
            {
                classes += " rare-item";
            }
            if (dungeon.Rarity > 1)
            {
                classes += " crafted-item";
            }
            return classes;
        }
    }
}
