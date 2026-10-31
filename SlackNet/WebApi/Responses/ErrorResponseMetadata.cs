using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ErrorResponseMetadata : SlackObject
{
    public IList<string> Messages { get; set; } = [];
}