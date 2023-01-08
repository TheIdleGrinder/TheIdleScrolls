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
        List<string> m_armorFamilies = new();

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
                    .Select(i => ItemFactory.GetItemFamilyIdFromName(i.GetComponent<ItemComponent>()!.FamilyName)!)
                    .ToList();
                m_armorFamilies = equipmentComp.GetItems()
                    .Where(i => i.IsItem() && i.IsArmor())
                    .Select(i => ItemFactory.GetItemFamilyIdFromName(i.GetComponent<ItemComponent>()!.FamilyName)!)
                    .ToList();
            }

            var multiplier = world.XpMultiplier * Math.Sqrt(world.Zone.Level);
            var gain = dt * multiplier;
            AddXP(m_weaponFamilies, gain, coordinator);
            AddXP(m_armorFamilies, gain, coordinator);

            m_firstUpdate = false;
        }

        void AddXP(List<string> itemFamilies, double fullAmount, Coordinator coordinator)
        {
            var abilitiesComp = m_player?.GetComponent<AbilitiesComponent>();
            if (abilitiesComp == null)
                return;
            var share = fullAmount / itemFamilies.Count; // Split experience among families of all equipped weapons

            foreach (string item in itemFamilies)
            {
                if (!m_timePerItemClass.ContainsKey(item))
                {
                    m_timePerItemClass[item] = 0.0;
                }
                m_timePerItemClass[item] += share;

                if (m_timePerItemClass[item] > 1.0) // A full XP has been reached
                {
                    int xp = (int)Math.Floor(m_timePerItemClass[item]);
                    m_timePerItemClass[item] -= xp;
                    var result = abilitiesComp.AddXP(item, xp);
                    if (result == AbilitiesComponent.AddXPResult.LevelIncreased)
                    {
                        var abilityName = ItemFactory.GetItemFamilyName(item) ?? item;
                        var newLevel = abilitiesComp.GetAbility(item)?.Level ?? 0;
                        coordinator.PostMessage(this, new AbilityImprovedMessage(abilityName, newLevel));
                    }
                }
            }
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

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.High;
        }
    }
}
