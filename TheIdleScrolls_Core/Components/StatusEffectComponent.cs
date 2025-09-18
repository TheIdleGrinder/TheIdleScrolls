using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.StatusEffects;

namespace TheIdleScrolls_Core.Components
{
	public class StatusEffectComponent : IComponent
	{
		readonly List<StatusEffect> _StatusEffects = [];

		public List<StatusEffect> StatusEffects => [.. _StatusEffects];

		public void Add(StatusEffect effect)
		{
			_StatusEffects.Add(effect);
		}

		public void Remove(StatusEffect effect)
		{
			_StatusEffects.Remove(effect);
		}

		public void DeactivateAll()
        {
            // CornerCut: There is a weird chain of "knowing who is owned by whom" going on here. Status effect removes itselt from the component
            // and might remove the component from the entity if it was the last one. The component on the other hand does not even know that 
            // it is owned by an entity. So we just deactivate all effects here, which will remove themselves from the component.
            foreach (var effect in StatusEffects)
            {
                effect.Deactivate();
            }
        }
    }
}
