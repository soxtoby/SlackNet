using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SlackNet.WebApi;

namespace SlackNet
{
    public class InteractiveMessage
    {
        public string Type { get; set; }
        public IList<Action> Actions { get; set; } = new List<Action>();
        [JsonIgnore]
        public Action Action
        {
            get => Actions.First();
            set => Actions = new List<Action> { value };
        }
        public string CallbackId { get; set; }
        public Team Team { get; set; }
        public Channel Channel { get; set; }
        public User User { get; set; }
        public string ActionTs { get; set; }
        public string MessageTs { get; set; }
        public string AttachmentId { get; set; }
        [JsonIgnore]
        public Attachment OriginalAttachment => OriginalMessage.Attachments.FirstOrDefault(a => a.Id == AttachmentId);
        public string Token { get; set; }
        public bool IsAppUnfurl { get; set; }
        public Message OriginalMessage { get; set; }
        public string ResponseUrl { get; set; }
        public string TriggerId { get; set; }
    }
}