using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniECS
{
    public class Coordinator
    {
        List<Entity> m_entities = new();
        List<ISystem> m_systems = new();
        MessageBoard m_messageBoard = new();

        public void AddEntity(Entity entity)
        {
            m_entities.Add(entity);
        }

        public bool RemoveEntity(uint id)
        {
            foreach (Entity entity in m_entities)
            {
                if (entity.Id == id)
                {
                    m_entities.Remove(entity);
                    return true;
                }
            }
            return false;
        }

        public bool HasEntity(uint id)
        {
            foreach (var entity in m_entities)
            {
                if (entity.Id == id)
                    return true;
            }
            return false;
        }

        public Entity? GetEntity(uint id)
        {
            foreach (var entity in m_entities)
            {
                if (entity.Id == id)
                    return entity;
            }
            return null;
        }

        public List<Entity> GetEntities()
        {
            return m_entities;
        }

        public List<Entity> GetEntities<T>() where T : class, IComponent
        {
            return m_entities.Where(e => e.HasComponent<T>()).ToList();
        }

        public List<Entity> GetEntities<T, U>() 
            where T : class, IComponent 
            where U : class, IComponent
        {
            return m_entities.Where(e => 
                e.HasComponent<T>() 
                && e.HasComponent<U>()).ToList();
        }

        public List<Entity> GetEntities<T, U, V>()
            where T : class, IComponent
            where U : class, IComponent
            where V : class, IComponent
        {
            return m_entities.Where(e =>
                e.HasComponent<T>()
                && e.HasComponent<U>()
                && e.HasComponent<V>()).ToList();
        }

        public void AddSystem(ISystem system)
        {
            m_systems.Add(system);
        }

        public T? GetSystem<T>() where T : class, ISystem
        {
            foreach(var system in m_systems)
            {
                if (system.GetType() == typeof(T))
                    return (T)system;
            }
            return null;
        }

        public void PostMessage(ISystem sender, IMessage message)
        {
            m_messageBoard.PostMessage(sender, message);
        }

        public List<IMessage> FetchAllMessages()
        {
            return m_messageBoard.GetAllMessages();
        }

        public List<T> FetchMessagesByType<T>() where T : class, IMessage
        {
            return m_messageBoard.GetMessagesOfType<T>();
        }

        public bool MessageTypeIsOnBoard<T>() where T : class, IMessage
        {
            return m_messageBoard.HasMessageOfType<T>();
        }

        public void DeleteMessagesFromSender(ISystem system)
        {
            m_messageBoard.DeleteMessagesFromSender(system);
        }
    }
}
