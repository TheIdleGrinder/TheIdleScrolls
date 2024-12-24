using TheIdleScrolls_Core.Definitions;

namespace TheIdleScrolls_Core.Items
{
    public interface IItemEntity
    {
        public uint Id { get; }
        public string Name { get; }
        public string Description { get; }
        public List<EquipmentSlot> Slots { get; }
        public int Quality { get; }
        public int Value { get; }
        public (int Cost, double Duration) Refining { get; }
        public bool Crafted { get; }
    }
}
