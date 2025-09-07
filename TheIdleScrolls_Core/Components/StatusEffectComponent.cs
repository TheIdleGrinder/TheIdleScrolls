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
	}
}
