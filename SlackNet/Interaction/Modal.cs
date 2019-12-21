using System.Collections.Generic;
using SlackNet.Blocks;

namespace SlackNet.Interaction
{
    public class Modal
    {
        
        private string _title;
        public Dictionary<string, string> Title
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"type", "plain_text"},
                    {"text", _title}
                };
            }
        }
        public readonly string Type = "modal";
        public string CallbackId { get; set; }
        public string SubmitLabel { get; set; }
        public string CancelLabel { get; set; }
        public string State { get; set; }
        public IList<Block> Blocks { get; set; } = new List<Block>();
        
        
        public Modal(string callbackId, string title)
        {
            CallbackId = callbackId;
            _title = title;
        }
    }
}