using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core.Systems
{
    public class AbilitiesSystem : AbstractSystem
    {
        bool m_firstUpdate = true;

        // CornerCut: Assume only the player entity has abilities
        Entity? m_player = null;

        List<string> m_weaponFamilies = new();

        Dictionary<string, double> m_timePerItemClass = new();

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == null)
                m_player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null || !m_player.HasComponent<AbilitiesComponent>())
                return;

            var inCombat = m_player.GetComponent<AttackComponent>()?.InCombat ?? false;
            if (!inCombat)
                return; // CornerCut: Assumes abilities only increase during combat

            var abilitiesComp = m_player.GetComponent<AbilitiesComponent>();
            if (abilitiesComp == null)
                return;

            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<ItemMovedMessage>())
            {
                ItemFactory itemFactory = new();
                var equipmentComp = m_player.GetComponent<EquipmentComponent>();
                if (equipmentComp == null)
                    return; // CornerCut: Assumes that all abilities are tied to items
                m_weaponFamilies = equipmentComp.GetItems()
                    .Where(i => i.IsItem() && i.IsWeapon())
                    .Select(i => itemFactory.GetItemFamilyIdFromName(i.GetComponent<ItemComponent>()!.FamilyName)!)
                    .ToList();
            }

            var multiplier = world.XpMultiplier * Math.Sqrt(world.AreaLevel);
            var share = dt / m_weaponFamilies.Count;
            foreach (var weapon in m_weaponFamilies)
            {
                if (!m_timePerItemClass.ContainsKey(weapon))
                {
                    m_timePerItemClass[weapon] = 0.0;
                }
                m_timePerItemClass[weapon] += share * multiplier;

                if (m_timePerItemClass[weapon] > 1.0)
                {
                    int xp = (int)Math.Floor(m_timePerItemClass[weapon]);
                    m_timePerItemClass[weapon] -= xp;
                    var result = abilitiesComp.AddXP(weapon, xp);
                    if (result == AbilitiesComponent.AddXPResult.LevelIncreased)
                    {
                        var newLevel = abilitiesComp.GetAbility(weapon)?.Level ?? 0;
                        coordinator.PostMessage(this, new AbilityImprovedMessage(weapon, newLevel));
                    }
                }
            }

            m_firstUpdate = false;
        }
    }

    public class AbilityImprovedMessage : IMessage
    {
        public string AbilityId { get; set; }

        public int NewLevel { get; set; }

        public AbilityImprovedMessage(string id, int newLevel)
        {
            AbilityId = id;
            NewLevel = newLevel;
        }

        string IMessage.BuildMessage()
        {
            return $"Ability {AbilityId} reached level {NewLevel}";
        }
    }
}
