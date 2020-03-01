using System.Collections.Generic;
using SlackNet.Blocks;

namespace SlackNet.Interaction
{
    public class ViewErrorsResponse : ViewSubmissionResponse
    {
        public ViewErrorsResponse() : base("errors") { }

        /// <summary>
        /// Supply a key that is the <see cref="Block.BlockId"/> of the erroneous input block,
        /// and a value - the plain text error message to be displayed to the user.
        /// </summary>
        public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    }
}