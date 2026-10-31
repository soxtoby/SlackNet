using System.Collections.Generic;

namespace SlackNet.WebApi;

public class FeaturedWorkflowsListResponse : SlackObject
{
    public IList<ChannelFeaturedWorkflows> FeaturedWorkflows { get; set; } = new List<ChannelFeaturedWorkflows>();
}

public class ChannelFeaturedWorkflows : SlackObject
{
    public string ChannelId { get; set; }
    public IList<FeaturedTrigger> Triggers { get; set; } = new List<FeaturedTrigger>();
}

public class FeaturedTrigger : SlackObject
{
    public string Id { get; set; }
    public string Title { get; set; }
}
