#nullable enable
namespace SlackNet;

/// <summary>
/// A user that could not be added to a usergroup by <see cref="WebApi.IAdminUserGroupsApi.AddUsers"/> or <see cref="WebApi.IAdminUserGroupsApi.UploadUsers"/>.
/// </summary>
public class UserGroupUserFailure
{
    public string UserId { get; set; } = null!;

    /// <summary>
    /// The reason the user could not be added, such as <c>guest_user</c>, <c>deleted_user</c> or <c>user_not_found</c>.
    /// </summary>
    public string Reason { get; set; } = null!;
}
