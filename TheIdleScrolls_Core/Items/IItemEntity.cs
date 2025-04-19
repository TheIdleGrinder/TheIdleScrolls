using MiniECS;
using TheIdleScrolls_Core.Definitions;

namespace TheIdleScrolls_Core.Items
{
    using CandidateComparator = Func<Entity, Entity, ComparisonResult>;

    public interface IItemEntity
    {
        public uint Id { get; }
        public string Name { get; }
        public string Type { get; }
        public string RelatedAbility { get; }
        public string Description { get; }
        public List<EquipmentSlot> Slots { get; }
        public double Encumbrance { get; }
        public int DropLevel { get; }
        public int Quality { get; }
        public int Value { get; }
        public (int Cost, double Duration) Refining { get; }
        public bool Crafted { get; }
        public WeaponGenus? WeaponAspect { get; }
        public ArmorGenus? ArmorAspect { get; }
        public bool IsEquipped { get; }

        public List<ComparisonResult> CompareToEquipment(CandidateComparator comparator);
    }
}
