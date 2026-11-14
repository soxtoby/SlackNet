#nullable enable
using System;

namespace SlackNet;

/// <summary>
/// Metadata about a public channel appearing in <see cref="PublicChannelAnalytics"/>.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.getFile/#metadata_only">Slack documentation</a> for more information.</remarks>
public class PublicChannelMetadata
{
    /// <summary>
    /// The channel's unique identifier; the very same used in <see cref="PublicChannelAnalytics.ChannelId"/>.
    /// </summary>
    public required string ChannelId { get; set; }

    /// <summary>
    /// The latest name of the channel.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The latest channel topic.
    /// </summary>
    public string? Topic { get; set; }

    /// <summary>
    /// A longer description about the purpose of the channel.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// These details are current as of this date, which is also when you're making this API call.
    /// </summary>
    public DateTime Date { get; set; }
}
