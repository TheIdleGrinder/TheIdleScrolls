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
				if (Skills.Count <= Index || Index < 0)
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
			} while (
				CurrentSkill?.GetState() != ActiveSkill.State.Ready
				&& CurrentSkill?.GetState() != ActiveSkill.State.Charging
				&& Index != originalIndex);
			if (CurrentSkill?.GetState() != ActiveSkill.State.Ready
                && CurrentSkill?.GetState() != ActiveSkill.State.Charging)
			{
				// No skill is ready to use
				Index = -1;
			}
            return Index;
		}

		public void ResetSkills()
		{
			foreach (var skill in Skills)
			{
				skill?.Timer.Reset();
			}
			// Select first available skill
			Index = -1;
			SwitchToNext();
		}

		public void Add(ActiveSkill skill)
		{
			if (!Skills.Any(s => s.Id == skill.Id))
			{
				Skills.Add(skill);
			}
		}

		public void SetSkillEnabled(string skillId, bool enabled)
        {
            var skill = Skills.FirstOrDefault(s => s.Id == skillId);
            if (skill != null)
            {
                skill.Enabled = enabled;
				if (!enabled && CurrentSkill == skill)
				{
					skill.Timer.Stop();
					SwitchToNext();
                }
				if (enabled && CurrentSkill == null)
				{
					SwitchToNext();
				}
            }
        }

        public void MoveSkillUp(ActiveSkill skill)
		{
			int idx = Skills.IndexOf(skill);
			if (idx > 0)
			{
				Skills.RemoveAt(idx);
				Skills.Insert(idx - 1, skill);
                // keep the same skill active
				if (idx == Index)
                    Index--;
                if (idx - 1 == Index && Index + 1 < Skills.Count)
                    Index++;
            }
        }

		public void MoveSkillDown(ActiveSkill skill)
		{
			int idx = Skills.IndexOf(skill);
			if (idx >= 0 && idx < Skills.Count - 1)
			{
				Skills.RemoveAt(idx);
				Skills.Insert(idx + 1, skill);
                // keep the same skill active
                if (idx == Index)
                    Index++;
                if (idx + 1 == Index && Index > 0)
                    Index--;
            }
		}
	}
}
