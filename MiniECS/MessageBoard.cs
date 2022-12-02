using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniECS
{
    internal class MessageBoard
    {
        List<KeyValuePair<ISystem, IMessage>> m_messages = new();

        public void PostMessage(ISystem sender, IMessage message)
        {
            m_messages.Add(new KeyValuePair<ISystem, IMessage>(sender, message));
        }

        public bool HasMessageOfType<T>() where T : IMessage
        {
            foreach (var message in m_messages)
            {
                if (message.Value.GetType() == typeof(T)) 
                    return true;
            }
            return false;
        }

        public List<T> GetMessagesOfType<T>() where T : IMessage
        {
            List<T> messages = new();
            foreach (var message in m_messages)
            {
                if (message.Value.GetType() == typeof(T))
                    messages.Add((T)message.Value);
            }
            return messages;
        }

        public List<IMessage> GetAllMessages()
        {
            return m_messages.Select(m => m.Value).ToList();
        }

        public List<IMessage> GetMessagesFromSender(ISystem sender)
        {
            List<IMessage> messages = new();
            foreach (var message in m_messages)
            {
                if (message.Key == sender)
                    messages.Add(message.Value);
            }
            return messages;
        }

        public void DeleteMessagesFromSender(ISystem sender)
        {
            for (int i = m_messages.Count - 1; i >= 0; i--)
            {
                var message = m_messages[i];
                if (message.Key == sender)
                    m_messages.RemoveAt(i);
            }
        }
    }
}
