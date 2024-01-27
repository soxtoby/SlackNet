using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// Provides a way to filter the list of options in a <see cref="ConversationSelectMenu"/> or <see cref="ConversationMultiSelectMenu"/>.
/// </summary>
public class ConversationFilter
{
    /// <summary>
    /// Indicates which type of conversations should be included in the list.
    /// When this field is provided, any conversations that do not match will be excluded.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<ConversationTypeFilter> Include { get; set; } = [];

    /// <summary>
    /// Indicates whether to exclude external shared channels from conversation lists. 
    /// </summary>
    public bool ExcludeExternalSharedChannels { get; set; }

    /// <summary>
    /// Indicates whether to exclude bot users from conversation lists.
    /// </summary>
    public bool ExcludeBotUsers { get; set; }
}

public enum ConversationTypeFilter
{
    Public,
    Private,
    Mpim,
    Im
}