using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Skills;

namespace TheIdleScrolls_Core.Components
{
	public class ActiveSkillComponent : IComponent
	{
		public List<ActiveSkill> Skills = [];
		int Index = 0;

		public ActiveSkill? CurrentSkill
		{
			get
			{
				if (Skills.Count <= Index)
					return null;
				return Skills[Index];
			}
		}

		public int SwitchToNext()
		{
			if (Skills.Count == 0)
				return -1;

			CurrentSkill?.Timer.Stop();

			int originalIndex = Index;
            do
			{
				Index = (Index + 1) % Skills.Count;
			} while (CurrentSkill?.CurrentState == SkillTimer.State.Cooldown && Index != originalIndex);
            return Index;
		}

		public void ResetSkills()
		{
			foreach (var skill in Skills)
			{
				skill?.Timer.Reset();
			}
			Index = 0;
		}

		public void Add(ActiveSkill skill)
		{
			if (!Skills.Any(s => s.Id == skill.Id))
			{
				Skills.Add(skill);
			}
		}

		public void MoveSkillUp(ActiveSkill skill)
		{
			int idx = Skills.IndexOf(skill);
			if (idx > 0)
			{
				Skills.RemoveAt(idx);
				Skills.Insert(idx - 1, skill);
			}
		}

		public void MoveSkillDown(ActiveSkill skill)
		{
			int idx = Skills.IndexOf(skill);
			if (idx >= 0 && idx < Skills.Count - 1)
			{
				Skills.RemoveAt(idx);
				Skills.Insert(idx + 1, skill);
			}
		}
	}
}
