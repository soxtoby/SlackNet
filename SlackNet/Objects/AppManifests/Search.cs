#nullable enable
using System.Collections.Generic;

namespace SlackNet.AppManifests;

/// <summary>
/// Settings that describe <a href="https://docs.slack.dev/enterprise-search/">enterprise search</a> configuration.
/// </summary>
public class Search
{
    /// <summary>
    /// The <c>callback_id</c> of the function used to provide filters for search.
    /// </summary>
    public string? SearchFiltersFunctionCallbackId { get; set; }

    /// <summary>
    /// The <c>callback_id</c> of the function used to search for items.
    /// </summary>
    public required string SearchFunctionCallbackId { get; set; }

    /// <summary>
    /// Settings that describe how AI interacts with your search connector.
    /// </summary>
    public SlackbotMetadata? SlackbotMetadata { get; set; }
}

/// <summary>
/// Settings that describe how AI interacts with your search connector.
/// </summary>
public class SlackbotMetadata
{
    public string? Description { get; set; }
    public IList<JsonData>? Examples { get; set; }
    public string? ExampleQuery { get; set; }
}
