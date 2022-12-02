using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniECS
{
    public interface IMessage
    {
        string Message { get { return BuildMessage(); } }

        protected string BuildMessage();
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
    }
}
