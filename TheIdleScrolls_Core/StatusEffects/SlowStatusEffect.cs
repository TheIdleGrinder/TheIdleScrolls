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
	public class SlowStatusEffect(double duration, double baseMagnitude) : StatusEffect("Slowed", duration)
	{
		List<Modifier> _Modifiers = [];
		public double BaseMagnitude { get; init; } = baseMagnitude;
        public double Magnitude { get; set; } = baseMagnitude;

        public override string Description => $"Slowed by {Magnitude:0.##%} for {Timer?.Duration ?? double.PositiveInfinity:0.##} sec.";

        public override double ScaleDurationForTarget(Entity target, double duration)
		{
			// CornerCut: Introducing a sneaky side effect here scaling magnitude here
			// Should probably rename the method at some point, let's see what is needed for other status effects
			double resist = target.ApplyAllApplicableModifiers(0.0, [Tags.Slow, Tags.Status, Tags.Debuff, Tags.Resistance], target.GetTags());
			resist = Math.Min(resist, 1.0);
			Magnitude = BaseMagnitude * (1.0 - resist);
            return duration;
		}

		protected override void ActivateEffect(Entity target)
		{
			string guid = Guid.NewGuid().ToString();
			var asMod = new Modifier($"slowed-{guid}", ModifierType.More, -Magnitude, [Tags.AttackSpeed], []);
			target.GetComponent<ModifierComponent>()?.AddModifier(asMod);

			_Modifiers = [asMod];
		}

		protected override void DeactivateEffect(Entity target)
		{
			var modComp = target.GetComponent<ModifierComponent>();
			if (modComp is null)
				return;
			_Modifiers.ForEach(m => modComp.RemoveModifier(m.Id));
		}
	}
}
