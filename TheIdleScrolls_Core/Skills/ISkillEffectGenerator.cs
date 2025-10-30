using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Skills
{
    public interface ISkillEffectGenerator
    {
        public string Description { get; }
        public void Reset();
        public List<ISkillEffect> Update(double dt);
    }
}
