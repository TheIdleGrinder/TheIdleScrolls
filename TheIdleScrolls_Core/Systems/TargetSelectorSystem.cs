using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Systems
{
    public class TargetSelectorSystem : AbstractSystem
    {
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            foreach (var entity in coordinator.GetEntities().Where(e => e.HasComponent<AttackComponent>()))
            {
                var attackComp = entity.GetComponent<AttackComponent>() ?? throw new Exception("Entity is missing AttackComponent");

                if (attackComp.Target != 0 && !coordinator.HasEntity(attackComp.Target)) // Target no longer exists
                {
                    attackComp.Target = 0;
                }

                if (attackComp.Target == 0)
                {
                    if (entity.HasComponent<PlayerComponent>())
                    {
                        var target = coordinator.GetEntities().FirstOrDefault(e => e.HasComponent<MobComponent>());
                        attackComp.Target = target?.Id ?? 0;

                        string attackerName = entity.GetComponent<NameComponent>()?.Name ?? $"Entity #{entity.Id}";
                        string targetName = target?.GetComponent<NameComponent>()?.Name ?? $"entity #{target?.Id ?? 0}" ?? "nothing";
                        string message = $"{attackerName} is now targeting {targetName}";
                        coordinator.PostMessage(this, new TextMessage(message));
                    }
                }
            }
        }
    }
}
