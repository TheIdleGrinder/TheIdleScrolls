using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;

namespace TheIdleScrolls_Core.StatusEffects
{
    public class RestingStatusEffect() : StatusEffect("Resting")
    {
        Modifier RegenModifier = new("resting_hpreg", ModifierType.AddBase, 0.0, [Tags.LifeRegeneration], []);
        int Stage = 1;
        double TotalDuration = 0.0;

        public override string Description => $"You are resting and regenerate health.";

        protected override void ActivateEffect(Entity target)
        {
            Stage = 1;
            TotalDuration = 0.0;
            double reg = CalculateRegen(target);
            RegenModifier.Value = reg;
            target.GetComponent<ModifierComponent>()?.AddModifier(RegenModifier);
        }

        protected override void DeactivateEffect(Entity target)
        {
            target.GetComponent<ModifierComponent>()?.RemoveModifier(RegenModifier.Id);
        }

        protected override void UpdateEffect(Entity target, double dt)
        {
            TotalDuration += dt;
            int newStage = (int)TotalDuration + 1;
            if (newStage != Stage)
            {
                Stage = newStage;
                double reg = CalculateRegen(target);
                RegenModifier.Value = reg;
            }
        }

        double CalculateRegen(Entity target)
        {
            double totalLife = target.GetComponent<LifePoolComponent>()?.Maximum ?? 0.0;
            return 0.01 * Stage * totalLife;
        }
    }
}
