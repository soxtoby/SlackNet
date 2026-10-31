using System.Collections.Generic;

namespace SlackNet.Interaction;

public class DialogErrorResponse : SlackObject
{
    public IEnumerable<DialogError> Errors { get; set; }
}