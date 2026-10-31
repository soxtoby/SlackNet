using System.Collections.Generic;

namespace SlackNet;

public class UnfurlMetadata : SlackObject
{
    public IList<WorkObjectEntity> Entities { get; set; } = [];
}