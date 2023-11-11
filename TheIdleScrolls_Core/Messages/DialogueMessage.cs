using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Messages
{
    public class DialogueMessage : IMessage
    {
        public string ResponseId { get; set; }
        public string Speaker { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public List<string> ResponseOptions { get; set; }

        public DialogueMessage(string responseId, string speaker, string title, string message, List<string> responseOptions)
        {
            ResponseId = responseId;
            Speaker = speaker;
            Title = title;
            Message = message;
            ResponseOptions = responseOptions;
        }

        string IMessage.BuildMessage()
        {
            return $"[{Title}@{ResponseId}]\n{Speaker}: {Message}" + ResponseOptions.Aggregate((agg, r) => $"{agg}\n- {r}");
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }
}
