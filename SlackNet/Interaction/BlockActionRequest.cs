using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SlackNet.Blocks;
using SlackNet.Events;

namespace SlackNet.Interaction
{
    [SlackType("block_actions")]
    public class BlockActionRequest : InteractionRequest
    {
        public Container Container { get; set; }
        public string TriggerId { get; set; }
        public MessageEvent Message { get; set; }
        public ViewInfo View { get; set; }
        public ViewState State { get; set; }
        public IList<BlockAction> Actions { get; set; } = new List<BlockAction>();
        [JsonIgnore]
        public BlockAction Action => Actions.First();
    }
}