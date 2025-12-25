using System.Collections.Generic;

namespace SlackNet.WebApi;

public class FeaturedWorkflowsListResponse
{
    public IList<ChannelFeaturedWorkflows> FeaturedWorkflows { get; set; } = new List<ChannelFeaturedWorkflows>();
}

public class ChannelFeaturedWorkflows
{
    public string ChannelId { get; set; }
    public IList<FeaturedTrigger> Triggers { get; set; } = new List<FeaturedTrigger>();
}

public class FeaturedTrigger
{
    public string Id { get; set; }
    public string Title { get; set; }
}
