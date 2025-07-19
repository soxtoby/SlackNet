namespace SlackNet;

/// <summary>
/// A user with access to a custom function.
/// </summary>
public class FunctionPermissionUser
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}
