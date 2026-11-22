namespace SlackNet;

/// <summary>
/// A user or entity whose role assignment could not be added or removed.
/// </summary>
public class RoleAssignmentRejection
{
    /// <summary>
    /// The ID of the user or entity.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The reason the ID was rejected.
    /// </summary>
    public required string Error { get; set; }
}
