using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniECS
{
    public class Entity
    {
        uint m_id = 0;

        List<IComponent> m_components = new List<IComponent>();

        static uint NextId = 1;

        public uint Id { get { return m_id; } }

        public List<IComponent> Components { get { return m_components.ToList(); } }

        public Entity()
        {
            m_id = NextId++;
        }

        public void AddComponent<T>(T component) where T : class, IComponent
        {
            if (HasComponent<T>())
                RemoveComponent<T>();
            m_components.Add(component);
        }

        public T? GetComponent<T>() where T : class, IComponent
        {
            foreach (var component in m_components)
            {
                if (component.GetType() == typeof(T))
                    return (T)component;
            }
            return null;
        }

        public bool HasComponent<T>() where T : class, IComponent
        {
            return GetComponent<T>() != null;
        }

        public bool RemoveComponent<T>() where T : IComponent
        {
            foreach (var component in m_components)
            {
                if (component.GetType() == typeof(T))
                {
                    m_components.Remove(component);
                    return true;
                }
            }
            return false;
        }
    }
}
