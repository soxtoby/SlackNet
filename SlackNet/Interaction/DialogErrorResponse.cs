using System.Collections.Generic;

namespace SlackNet.Interaction
{
    public class DialogErrorResponse
    {
        public IEnumerable<DialogError> Errors { get; set; }
    }
}