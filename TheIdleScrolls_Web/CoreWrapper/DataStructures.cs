using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Web.CoreWrapper
{
    public class TimeLimit
    {
        public double Remaining { get; set; } = 0.0;
        public double Maximum { get; set; } = 0.0;
    }

    public class AccessibleAreas
    {
        public int MaxWilderness { get; set; } = 0;
        public List<DungeonRepresentation> Dungeons { get; set; } = new();
    }

    public class CharacterStats
    {
        public DamageCluster Damage { get; set; } = new();
        public double CooldownRemaining { get; set; } = 0.0;
        public double Cooldown { get; set; } = 0.0;
        public double Armor { get; set; } = 0.0;
        public double Evasion { get; set; } = 0.0;
        public double DefenseRating { get; set; } = 0.0;
        public double Encumbrance { get; set; } = 0.0;
    }
}
