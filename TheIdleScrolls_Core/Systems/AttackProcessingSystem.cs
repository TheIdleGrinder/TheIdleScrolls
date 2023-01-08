using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using static System.Net.Mime.MediaTypeNames;

namespace TheIdleScrolls_Core.Systems
{
    public class AttackProcessingSystem : AbstractSystem
    {
        DamageDoneMessage? ApplyAttack(Entity attacker, Entity target)
        {
            var attackComp = attacker.GetComponent<AttackComponent>() ?? throw new Exception("Missing attack component");
            LifePoolComponent? hpComp = target.GetComponent<LifePoolComponent>();
            if (hpComp == null)
                return null;
            if (hpComp.Current == 0)
                return null; // Skip damage if target is already dead
            int damage = (int)Math.Round(attackComp.RawDamage, 0);
            hpComp.ApplyDamage(damage);
            return new DamageDoneMessage(attacker, target, damage);
        }

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            foreach (var entity in coordinator.GetEntities<AttackComponent>())
            {
                var attackComp = entity.GetComponent<AttackComponent>() 
                    ?? throw new Exception("AttackProcessingSystem: Missing AttackComponent in entity");
                
                var target = coordinator.GetEntity(attackComp.Target);
                if (target == null)
                {
                    continue;
                }

                int attacks = attackComp.Cooldown.Update(dt);
                for (int i = 0; i < attacks; i++)
                {
                    var message = ApplyAttack(entity, target);
                    if (message != null)
                        coordinator.PostMessage(this, message);
                }

                LifePoolComponent? hpComp = target.GetComponent<LifePoolComponent>();
                if (hpComp == null)
                    continue;
                if (hpComp.Current <= 0)
                {
                    coordinator.PostMessage(this, new DeathMessage(target));
                    target.AddComponent(new KilledComponent { Killer = entity.Id });
                }
            }
        }
    }

    public class DamageDoneMessage : IMessage
    {
        public Entity Attacker { get; set; }
        public Entity Target { get; set; }
        public int Damage { get; set; }

        public DamageDoneMessage(Entity attacker, Entity target, int damage)
        {
            Attacker = attacker;
            Target = target;
            Damage = damage;
        }

        string IMessage.BuildMessage()
        {
            string attackerName = Attacker.GetName();
            string targetName = Target.GetName();
            int remainingHP = Target.GetComponent<LifePoolComponent>()?.Current ?? 0;
            int fullHP = Target.GetComponent<LifePoolComponent>()?.Maximum ?? 0;
            return $"{attackerName} did {Damage} damage to {targetName} ({remainingHP}/{fullHP} HP remaining)";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.VeryLow;
        }
    }

    public class DeathMessage : IMessage
    {
        public Entity Victim { get; set; }

        public DeathMessage(Entity victim)
        {
            Victim = victim;
        }

        string IMessage.BuildMessage()
        {
            return $"{Victim.GetName()} was defeated";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Medium;
        }
    }
}
