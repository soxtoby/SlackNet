namespace SlackNet;

/// <summary>
/// A permission scope requested by an app.
/// </summary>
public class AppScope
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsSensitive { get; set; }

    /// <summary>
    /// The type of token the scope applies to, e.g. <c>bot</c> or <c>user</c>.
    /// </summary>
    public required string TokenType { get; set; }
    public bool IsOptional { get; set; }
    public bool IsApproved { get; set; }
}
