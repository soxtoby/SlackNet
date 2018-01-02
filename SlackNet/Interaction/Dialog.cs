using System.Collections.Generic;

namespace SlackNet.Interaction
{
    public class Dialog
    {
        public string CallbackId { get; set; }
        public string Title { get; set; }
        public string SubmitLabel { get; set; }
        public IList<DialogElement> Elements { get; set; } = new List<DialogElement>();
    }
}