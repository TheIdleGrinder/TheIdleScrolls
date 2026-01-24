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
	public class StunStatusEffect(double duration) : StatusEffect("Stunned", duration)
	{
		List<Modifier> _Modifiers = [];

        public override string Description => $"Stun with {Timer?.Duration ?? double.PositiveInfinity:0.##} sec. duration";

        public override double ScaleDurationForTarget(Entity target, double duration)
		{
			double resist = target.ApplyAllApplicableModifiers(0.0, [Tags.Stun, Tags.Status, Tags.Resistance], target.GetTags());
			resist = Math.Min(resist, 1.0);
			return duration * (1.0 - resist);
		}

		protected override void ActivateEffect(Entity target)
		{
			string guid = Guid.NewGuid().ToString();
			var asMod = new Modifier($"stun-{guid}", ModifierType.More, -1.0, [Tags.ChargeSpeed], []);
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
