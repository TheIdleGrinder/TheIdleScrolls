using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using static System.Net.Mime.MediaTypeNames;

namespace TheIdleScrolls_Core.Systems
{
    /// <summary>
    /// Deprecated: This system is no longer used in the game, attacks are now processed in the BattleSystem
    /// </summary>
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
            // Deactivated: No longer compiles
            //foreach (var entity in coordinator.GetEntities<AttackComponent>())
            //{
            //    var attackComp = entity.GetComponent<AttackComponent>() 
            //        ?? throw new Exception("AttackProcessingSystem: Missing AttackComponent in entity");
                
            //    var target = coordinator.GetEntity(attackComp.Target);
            //    if (target == null)
            //    {
            //        continue;
            //    }

            //    int attacks = attackComp.Cooldown.Update(dt);
            //    for (int i = 0; i < attacks; i++)
            //    {
            //        var message = ApplyAttack(entity, target);
            //        if (message != null)
            //            coordinator.PostMessage(this, message);
            //        attackComp.PerformAttack(); // Track attack count in current battle
            //    }

            //    LifePoolComponent? hpComp = target.GetComponent<LifePoolComponent>();
            //    if (hpComp == null)
            //        continue;
            //    if (hpComp.Current <= 0)
            //    {
            //        coordinator.PostMessage(this, new DeathMessage(target));
            //        target.AddComponent(new KilledComponent { Killer = entity.Id });
            //    }
            //}
        }
    }
}
