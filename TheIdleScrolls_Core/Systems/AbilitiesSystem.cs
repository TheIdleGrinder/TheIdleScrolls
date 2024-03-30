﻿using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core.Systems
{
    public class AbilitiesSystem : AbstractSystem
    {
        const double BaseXpMultiplier = 2.0;
        const double RarityXpMultiplier = 1.1;

        bool m_firstUpdate = true;

        // CornerCut: Assume only the player entity has abilities
        Entity? m_player = null;

        List<string> m_weaponFamilies = new();
        List<string> m_armorFamilies = new();

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
            var craftAbl = abilitiesComp.GetAbility(Properties.Constants.Key_Ability_Crafting);
            if (craftAbl != null)
            {
                bool levelIncrease = false;
                foreach (var reforgeMsg in coordinator.FetchMessagesByType<CraftingStartedMessage>().Where(m => m.Owner == m_player))
                {
                    // Base value is 1 XP per coin paid
                    double xp = ApplyModifiers(Properties.Constants.Key_Ability_Crafting, reforgeMsg.CoinsPaid);
                    xp *= BaseXpMultiplier * world.XpMultiplier;
                    AbilitiesComponent.AddXPResult result = abilitiesComp.AddXP(Properties.Constants.Key_Ability_Crafting, (int)xp);
                    if (result == AbilitiesComponent.AddXPResult.LevelIncreased)
                        levelIncrease = true;
                }
                if (levelIncrease)
                    coordinator.PostMessage(this, new AbilityImprovedMessage(craftAbl.Key.Localize(), craftAbl.Level));
            }

            // Update fighting abilities
            if (m_player.GetComponent<AttackComponent>()?.InCombat ?? false)
            {
                if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<ItemMovedMessage>())
                {
                    var equipmentComp = m_player.GetComponent<EquipmentComponent>();
                    if (equipmentComp == null)
                        return; // CornerCut: Assumes that all abilities are tied to items
                    m_weaponFamilies = equipmentComp.GetItems()
                        .Where(i => i.IsItem() && i.IsWeapon())
                        .Select(i => i.GetComponent<ItemComponent>()!.Code.FamilyId) // ! is ok, because the entity is guaranteed to be an item
                        .ToList();
                    m_armorFamilies = equipmentComp.GetItems()
                        .Where(i => i.IsItem() && i.IsArmor())
                        .Select(i => i.GetComponent<ItemComponent>()!.Code.FamilyId) // ! is ok, because the entity is guaranteed to be an item
                        .ToList();
                }

                int zoneLevel = m_player.GetComponent<LocationComponent>()?.GetCurrentZone(world.Map)?.Level ?? 1;
                var multiplier = world.XpMultiplier * Math.Sqrt(zoneLevel);
                var gain = dt * multiplier;
                AddXP(m_weaponFamilies, gain, coordinator);
                AddXP(m_armorFamilies, gain, coordinator);

                m_firstUpdate = false; // First update is not relevant for crafting etc., only for the fighting abilities
            }
        }

        void AddXP(List<string> itemFamilies, double fullAmount, Coordinator coordinator)
        {
            var abilitiesComp = m_player?.GetComponent<AbilitiesComponent>();
            if (abilitiesComp == null)
                return;
            var share = fullAmount / itemFamilies.Count; // Split experience among families of all equipped weapons/armors

            foreach (string item in itemFamilies)
            {
                if (!m_timePerItemClass.ContainsKey(item))
                {
                    m_timePerItemClass[item] = 0.0;
                }
                double scaledShare = ApplyModifiers(item, share);
                m_timePerItemClass[item] += scaledShare;

                if (m_timePerItemClass[item] > 1.0) // A full XP has been reached
                {
                    int xp = (int)Math.Floor(m_timePerItemClass[item]);
                    m_timePerItemClass[item] -= xp;
                    var result = abilitiesComp.AddXP(item, xp);
                    if (result == AbilitiesComponent.AddXPResult.LevelIncreased)
                    {
                        var ability = abilitiesComp.GetAbility(item);
                        if (ability == null)
                            continue;
                        coordinator.PostMessage(this, new AbilityImprovedMessage(ability.Key.Localize(), ability.Level));
                    }
                }
            }
        }

        double ApplyModifiers(string ability, double baseValue)
            => m_player?.GetComponent<ModifierComponent>()
                ?.ApplyApplicableModifiers(baseValue, new string[] { Definitions.Tags.AbilityXpGain, ability }) ?? baseValue;
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
