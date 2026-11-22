#nullable disable
namespace SlackNet.WebApi;

/// <summary>
/// A type of channel to include or exclude in <see cref="IAdminConversationsApi.Search"/>.
/// </summary>
public enum ConversationSearchType
{
    /// <summary>
    /// Search private channels.
    /// </summary>
    Private,

    /// <summary>
    /// Exclude private channels from the search.
    /// </summary>
    PrivateExclude,

    Archived,
    ExcludeArchived,
    PrivateExcludeArchived,
    MultiWorkspace,
    OrgWide,
    ExternalSharedExclude,
    ExternalShared,
    ExternalSharedPrivate,
    ExternalSharedArchived,
    ExcludeOrgShared
}
