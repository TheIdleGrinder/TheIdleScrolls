using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Modifiers;

namespace TheIdleScrolls_Core.StatusEffects
{
    public class GenericModifierStatusEffect(
        string name, double duration, List<Modifier> modifiers, List<string> durationScalingTags) 
        : StatusEffect(name, duration)
    {
        public readonly List<Modifier> Modifiers = modifiers;
        public readonly List<string> DurationScalingTags = durationScalingTags;
        public override string Description { 
            get 
            {
                string mods = String.Join("\n", Modifiers.Select(m => "\t" + m.ToPrettyString()));
                if (Timer is not null)
                {
                    return $"For {Timer.Duration:0.##} seconds:" + mods;
                }
                else
                {
                    return mods;
                }
            } 
        }

        protected override void ActivateEffect(Entity target)
        {
            var modComp = target.GetComponent<ModifierComponent>();
            if (modComp is null)
            {
                modComp = new();
                target.AddComponent(modComp);
            }
            Modifiers.ForEach(m => modComp.AddModifier(m));
        }

        protected override void DeactivateEffect(Entity target)
        {
            var modComp = target.GetComponent<ModifierComponent>();
            if (modComp is null)
                return;
            Modifiers.ForEach(m => modComp.RemoveModifier(m.Id));
        }
    }
}
