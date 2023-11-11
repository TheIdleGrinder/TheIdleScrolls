using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Messages
{
    public class DialogueResponseMessage : IMessage
    {
        public string ResponseId { get; set; }
        public string Response { get; set; }

        public DialogueResponseMessage(string responseId, string response)
        {
            ResponseId = responseId;
            Response = response;
        }

        string IMessage.BuildMessage()
        {
            return $"Response @{ResponseId}: {Response}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }
}
