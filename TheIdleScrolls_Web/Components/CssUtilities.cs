using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Web.Components
{
    public class CssUtilities
    {
        public static string GetClassesForItem(IItemEntity? item, int? rarityOverride = null)
        {
            string classes = "";

            int rarity = rarityOverride ?? item?.Rarity ?? -1;
            if (item == null || rarity < 0)
            {
                classes += " rarity-none";
            }
            else
            {
                classes += $" rarity-{rarity}";
                if (rarity > 0)
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

    public static class ExtensionMethods
    {
        public static string ToBigIntString(this int value)
        {
            if (value < 1e5)
                return value.ToString();

            int exponent = (int)Math.Log10(value);
            string suffix = new string[] { "", "K", "M", "B" } [exponent / 3];
            int remainder = exponent % 3;
            int baseExponent = exponent - remainder;
            double fraction = value / Math.Pow(10, baseExponent);
            return remainder switch
            {
                0 => $"{fraction:0.##}",
                1 => $"{fraction:0.#}",
                _ => $"{fraction:0}"
            } + suffix;
        }

        public static string ToBigIntString(this double value)
        {
            if (value < 1e5 || value > Int32.MaxValue)
                return $"{value:0.##}";

            return ((int)value).ToBigIntString();
        }
    }
}
