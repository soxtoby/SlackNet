using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SlackNet.WebApi;

namespace SlackNet.Interaction
{
    public class InteractiveMessage : InteractionRequest
    {
        public string CallbackId { get; set; }
        public IList<ActionElement> Actions { get; set; } = new List<ActionElement>();
        [JsonIgnore]
        public ActionElement Action
        {
            get => Actions.First();
            set => Actions = new List<ActionElement> { value };
        }
        public string MessageTs { get; set; }
        public string AttachmentId { get; set; }
        [JsonIgnore]
        public Attachment OriginalAttachment => OriginalMessage.Attachments.FirstOrDefault(a => a.Id == AttachmentId);
        public bool IsAppUnfurl { get; set; }
        public Message OriginalMessage { get; set; }
        public string TriggerId { get; set; }
    }
}