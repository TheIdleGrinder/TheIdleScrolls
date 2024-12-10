using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core.Systems
{
    public class AbilitiesSystem : AbstractSystem
    {
        const double BaseXpMultiplier = 2.0;

        bool m_firstUpdate = true;

        // CornerCut: Assume only the player entity has abilities
        Entity? m_player = null;

        List<string> m_weaponAbilities = new();
        List<string> m_armorAbilities = new();

        readonly Dictionary<string, double> m_timePerItemClass = new();

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null || !m_player.HasComponent<AbilitiesComponent>())
                return;

            var abilitiesComp = m_player.GetComponent<AbilitiesComponent>();
            if (abilitiesComp == null)
                return;

            // Update crafting ability
            var craftAbl = abilitiesComp.GetAbility(Abilities.Crafting);
            if (craftAbl != null)
            {
                bool levelIncrease = false;
                foreach (var craftingMessage in coordinator.FetchMessagesByType<CraftingProcessFinished>().Where(m => m.Owner == m_player))
                {
                    const double xpPerCoin = 2.0;
                    double xp = ApplyModifiers(Abilities.Crafting, xpPerCoin * craftingMessage.Craft.CoinsPaid);
                    xp *= BaseXpMultiplier * world.XpMultiplier;
                    AbilitiesComponent.AddXPResult result = abilitiesComp.AddXP(Abilities.Crafting, (int)xp);
                    if (result == AbilitiesComponent.AddXPResult.LevelIncreased)
                        levelIncrease = true;
                }
                if (levelIncrease)
                    coordinator.PostMessage(this, new AbilityImprovedMessage(craftAbl.Key.Localize(), craftAbl.Level));
            }

            // Update fighting abilities if player is currently in battle
            if (m_player.HasComponent<BattlerComponent>())
            {
                if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<ItemMovedMessage>())
                {
                    var equipmentComp = m_player.GetComponent<EquipmentComponent>();
                    if (equipmentComp == null)
                        return; // CornerCut: Assumes that all abilities are tied to items
                    m_weaponAbilities = equipmentComp.GetItems()
                        .Where(i => i.IsItem() && i.IsWeapon())
                        .Select(i => i.GetComponent<ItemComponent>()!.Blueprint.GetFamilyDescription().RelatedAbilityId) // ! is ok, because the entity is guaranteed to be an item
                        .ToList();
                    if (m_weaponAbilities.Count == 0)
                        m_weaponAbilities.Add(Abilities.Unarmed);
                    m_armorAbilities = equipmentComp.GetItems()
                        .Where(i => i.IsItem() && i.IsArmor())
                        .Select(i => i.GetComponent<ItemComponent>()!.Blueprint.GetFamilyDescription().RelatedAbilityId) // ! is ok, because the entity is guaranteed to be an item
                        .ToList();
                    if (m_armorAbilities.Count == 0)
                        m_armorAbilities.Add(Abilities.Unarmored);
                }

                int zoneLevel = m_player.GetComponent<LocationComponent>()?.GetCurrentZone(world.Map)?.Level ?? 1;
                var multiplier = world.XpMultiplier * Math.Sqrt(zoneLevel);
                var gain = dt * multiplier;
                AddXP(m_weaponAbilities, gain, coordinator);
                AddXP(m_armorAbilities, gain, coordinator);

                m_firstUpdate = false; // First update is not relevant for crafting etc., only for the fighting abilities
            }
        }

        void AddXP(List<string> abilityIds, double fullAmount, Coordinator coordinator)
        {
            var abilitiesComp = m_player?.GetComponent<AbilitiesComponent>();
            if (abilitiesComp == null)
                return;
            var share = fullAmount / abilityIds.Count; // Split experience among families of all equipped weapons/armors

            foreach (string abilityId in abilityIds)
            {
                if (!m_timePerItemClass.ContainsKey(abilityId))
                {
                    m_timePerItemClass[abilityId] = 0.0;
                }
                double scaledShare = ApplyModifiers(abilityId, share);
                m_timePerItemClass[abilityId] += scaledShare;

                if (m_timePerItemClass[abilityId] > 1.0) // A full XP has been reached
                {
                    int xp = (int)Math.Floor(m_timePerItemClass[abilityId]);
                    m_timePerItemClass[abilityId] -= xp;
                    var result = abilitiesComp.AddXP(abilityId, xp);
                    if (result == AbilitiesComponent.AddXPResult.LevelIncreased)
                    {
                        var ability = abilitiesComp.GetAbility(abilityId);
                        if (ability == null)
                            continue;
                        coordinator.PostMessage(this, new AbilityImprovedMessage(ability.Key.Localize(), ability.Level));
                    }
                }
            }
        }

        double ApplyModifiers(string ability, double baseValue)
            => m_player?.GetComponent<ModifierComponent>()
                ?.ApplyApplicableModifiers(baseValue, 
                    new string[] { Definitions.Tags.AbilityXpGain, ability }, 
                    m_player?.GetTags() ?? new()) 
            ?? baseValue;
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
            return $"Ability '{AbilityId}' reached level {NewLevel}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.High;
        }
    }
}
