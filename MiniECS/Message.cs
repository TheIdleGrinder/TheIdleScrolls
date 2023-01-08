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
        string m_message;

        public TextMessage(string message)
        {
            m_message = message;
        }

        string IMessage.BuildMessage()
        {
            return m_message;
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }
}
