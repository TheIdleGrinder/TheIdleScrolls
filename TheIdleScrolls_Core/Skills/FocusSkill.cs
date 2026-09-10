using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Skills
{
    public static class FocusSkill
    {
        public static List<string> ActiveFocusSkills(Entity user, ActiveSkill? ignore)
        {
            var skillsComp = user.GetComponent<ActiveSkillComponent>();
            if (skillsComp == null)
                return [];
            return skillsComp.Skills
                .Where(skill => skill.SkillTags.Contains(Definitions.Tags.FocusSkill) 
                    && skill.CurrentState == SkillTimer.State.Active && !(ignore?.Equals(skill) ?? false))
                .Select(skill => skill.Id)
                .ToList();
        }

        public static int ActiveFocusSkillLimit(Entity user)
        {
            if (user is null)
                return 0;
            return (int)user.ApplyAllApplicableModifiers(1.0, [Definitions.Tags.ActiveFocusSkillLimit], user.GetTags());
        }
    }
}
