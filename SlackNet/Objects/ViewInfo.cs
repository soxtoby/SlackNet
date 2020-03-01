using System.Collections.Generic;
using SlackNet.Blocks;

namespace SlackNet
{
    public abstract class ViewInfo
    {
        public string Id { get; set; }
        public string TeamId { get; set; }
        public string Type { get; set; }
        public IList<Block> Blocks { get; set; } = new List<Block>();
        public string PrivateMetadata { get; set; }
        public string CallbackId { get; set; }
        public string ExternalId { get; set; }
        public ViewState State { get; set; }
        public string Hash { get; set; }
        public string RootViewId { get; set; }
        public string AppId { get; set; }
        public string BotId { get; set; }
        public string AppInstalledTeamId { get; set; }
    }

    public class ViewState
    {
        /// <summary>
        /// Keyed with the <see cref="Block.BlockId"/>s of any user-modified <see cref="InputBlock"/> blocks from the modal view.
        /// For each <see cref="Block.BlockId"/> is another dictionary keyed by the <see cref="IActionElement.ActionId"/>
        /// of the child <see cref="IInputBlockElement"/> of the input block.
        /// This final child object will contain the type and submitted value of the input block element.
        /// </summary>
        public Dictionary<string, Dictionary<string, ElementValue>> Values { get; set; } = new Dictionary<string, Dictionary<string, ElementValue>>();
    }

    public class ElementValue
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}