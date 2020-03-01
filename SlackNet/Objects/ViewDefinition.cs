using System.Collections.Generic;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace SlackNet
{
    public abstract class ViewDefinition
    {
        protected ViewDefinition(string type) => Type = type;
        
        public string Type { get; set; }

        /// <summary>
        /// A list of blocks that defines the content of the view.
        /// </summary>
        public IList<Block> Blocks { get; set; } = new List<Block>();

        /// <summary>
        /// An optional string that will be sent to your app in <see cref="ViewSubmission"/> and <see cref="BlockActionRequest"/> events. 
        /// </summary>
        public string PrivateMetadata { get; set; }

        /// <summary>
        /// An identifier to recognize interactions and submissions of this particular view.
        /// Don't use this to store sensitive information (use <see cref="PrivateMetadata"/> instead).
        /// </summary>
        public string CallbackId { get; set; }

        /// <summary>
        /// A custom identifier that must be unique for all views on a per-team basis.
        /// </summary>
        public string ExternalId { get; set; }
    }
}