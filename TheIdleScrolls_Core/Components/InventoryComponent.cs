using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public class InventoryComponent : IComponent
    {
        List<Entity> m_items = new();

        public int ItemCount { get { return m_items.Count; } }

        public void AddItem(Entity item)
        {
            if (!item.HasComponent<ItemComponent>())
            {
                throw new Exception($"Entity {item.GetName()} has no 'Item' component");
            }
            if (!m_items.Any(i => i.Id == item.Id))
                m_items.Add(item);
        }

        public bool RemoveItem(Entity item)
        {
            return m_items.Remove(item);
        }

        public List<Entity> GetItems()
        {
            return m_items.ToList();
        }
    }
}
