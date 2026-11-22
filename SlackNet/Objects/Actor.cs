namespace SlackNet;

/// <summary>
/// Who performed an action, such as approving or restricting an app, or approving or denying an invite request.
/// </summary>
public class Actor
{
    public required string ActorId { get; set; }

    /// <summary>
    /// The type of actor, such as <c>user</c> or <c>app</c>.
    /// </summary>
    public required string ActorType { get; set; }
}
