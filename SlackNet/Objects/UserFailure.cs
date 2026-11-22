namespace SlackNet;

/// <summary>
/// A user that an admin API method rejected,
/// such as one that could not be added by <see cref="WebApi.IAdminUserGroupsApi.AddUsers"/> or <see cref="WebApi.IAdminUserGroupsApi.UploadUsers"/>,
/// or one whose access could not be paused or resumed by <see cref="WebApi.IAdminUsersAccessApi"/>.
/// </summary>
public class UserFailure
{
    public required string UserId { get; set; }

    /// <summary>
    /// The reason the user was rejected, such as <c>guest_user</c>, <c>deleted_user</c>, <c>user_not_found</c> or <c>user_not_frontline_member</c>.
    /// </summary>
    public required string Reason { get; set; }
}
