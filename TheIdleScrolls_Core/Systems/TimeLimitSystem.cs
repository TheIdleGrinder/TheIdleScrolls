﻿using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Systems
{
    internal class TimeLimitSystem : AbstractSystem
    {
        Entity? m_player = null;

        bool m_inCombat = false;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == null)
                m_player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;

            var defComp = m_player.GetComponent<DefenseComponent>();

            bool previouslyInCombat = m_inCombat;
            m_inCombat = coordinator.GetEntities<MobComponent>().Count > 0;

            if (coordinator.MessageTypeIsOnBoard<MobSpawnMessage>()) // Combat just started, prepare time limit
            {
                int level = m_player.GetLevel();

                double evasion = defComp?.Evasion ?? 0.0;
                double evasionBonus = 1.0 + evasion / 100.0;

                double duration = 10.0 * level * evasionBonus / world.AreaLevel;
                world.TimeLimit.Reset(duration);
                coordinator.PostMessage(this, new TextMessage($"New time limit: {duration:0.###} s"));
            }

            if (m_inCombat)
            {
                var attackers = coordinator.GetEntities<MobDamageComponent>()
                    .Select(m => m.GetComponent<MobDamageComponent>()?.Multiplier ?? 1.0);
                if (attackers.Any())
                {
                    double armor = defComp?.Armor ?? 0.0;
                    double armorBonus = 1.0 + armor / 100.0;

                    var multi = attackers.Average();
                    world.TimeLimit.Update(multi * dt / armorBonus); // armor 'slows time'

                    if (world.TimeLimit.HasFinished) // Player lost the fight
                    {
                        var mobName = coordinator.GetEntities<MobComponent>().FirstOrDefault()?.GetName() ?? "??";
                        coordinator.PostMessage(this, new BattleLostMessage(m_player, mobName, world.AreaLevel));

                        coordinator.GetEntities<MobComponent>().ForEach(e => coordinator.RemoveEntity(e.Id)); // Despawn all mobs
                        m_inCombat = false;
                    }
                }
            }
        }
    }

    internal class BattleLostMessage : IMessage
    {
        public Entity Player;

        public string MobName;

        public int Level;

        public BattleLostMessage(Entity player, string mobName, int level)
        {
            Player = player;
            MobName = mobName;
            Level = level;
        }

        string IMessage.BuildMessage()
        {
            return $"{Player.GetName()} lost the fight against {MobName} (Level {Level})";
        }
    }
}
