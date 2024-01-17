using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public class TagsComponent : IComponent
    {
        private HashSet<string> m_tags = new();

        public TagsComponent() { }

        public TagsComponent(IEnumerable<string> tags)
        {
            AddTags(tags);
        }

        public bool HasTag(string tag) => m_tags.Contains(tag);

        public bool AddTag(string tag) => m_tags.Add(tag);

        public void AddTags(IEnumerable<string> tags)
        {
            foreach (string tag in tags)
            {
                m_tags.Add(tag);
            }
        }

        public bool RemoveTag(string tag) => m_tags.Remove(tag);

        public List<string> ListTags() => m_tags.OrderBy(x => x).ToList();


    }
}
