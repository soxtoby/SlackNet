using System.Collections.Generic;

namespace SlackNet.WebApi;

public class FunctionListResponse
{
    public IList<Function> Functions { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
