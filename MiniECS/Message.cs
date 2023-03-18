using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniECS
{
    public interface IMessage
    {
        public enum PriorityLevel { VeryHigh, High, Medium, Low, VeryLow, Debug }

        string Message { get { return BuildMessage(); } }

        PriorityLevel Priority { get { return GetPriority(); } }

        protected string BuildMessage();

        protected PriorityLevel GetPriority();
    }

    public class TextMessage : IMessage
    {
        readonly string m_message;
        
        readonly IMessage.PriorityLevel m_priorityLevel;

        public TextMessage(string message, IMessage.PriorityLevel priority = IMessage.PriorityLevel.Debug)
        {
            m_message = message;
            m_priorityLevel = priority;
        }

        string IMessage.BuildMessage()
        {
            return m_message;
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return m_priorityLevel;
        }
    }
}
