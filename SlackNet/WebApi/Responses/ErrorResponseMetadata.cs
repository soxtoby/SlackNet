using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ErrorResponseMetadata
{
    public IList<string> Messages { get; set; } = new List<string>();
}