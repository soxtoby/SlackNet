using System.Collections.Generic;

namespace SlackNet.Interaction
{
    public class MessageActionResponse
    {
        public string Text { get; set; }
        public IList<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ResponseType ResponseType { get; set; }
    }
}