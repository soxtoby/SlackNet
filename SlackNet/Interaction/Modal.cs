using System.Collections.Generic;
using SlackNet.Blocks;

namespace SlackNet.Interaction
{
    public class Modal
    {
        public readonly string Type = "modal";

        public string CallbackId { get; }

        public PlainText Title { get; set; }

        public PlainText Submit { get; set; }

        public PlainText Close { get; set; }

        public IList<Block> Blocks { get; set; } = new List<Block>();

        public string State { get; set; }

        public Modal(string callbackId)
        {
            CallbackId = callbackId;
        }
    }
}