#nullable enable
namespace SlackNet;

/// <summary>
/// An active login session to an Enterprise organization, as listed by <see cref="WebApi.IAdminUsersSessionApi.List"/>.
/// </summary>
public class ActiveSession
{
    /// <summary>
    /// Encoded ID of the user.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Encoded ID of the team.
    /// </summary>
    public required string TeamId { get; set; }

    /// <summary>
    /// ID of the session.
    /// </summary>
    public long SessionId { get; set; }

    /// <summary>
    /// The most recent version of the session.
    /// Only present if the session has changed since it was created, such as when it persists across multiple Slack client versions, OS versions or IPs.
    /// </summary>
    public SessionDetails? Recent { get; set; }

    /// <summary>
    /// The original version of the session.
    /// </summary>
    public SessionDetails Created { get; set; } = new();
}

/// <summary>
/// The device and client used by an <see cref="ActiveSession"/>.
/// </summary>
public class SessionDetails
{
    /// <summary>
    /// The type of device for the session.
    /// </summary>
    public string? DeviceHardware { get; set; }

    /// <summary>
    /// The operating system for the device.
    /// </summary>
    public string? Os { get; set; }

    /// <summary>
    /// The version of the OS.
    /// </summary>
    public string? OsVersion { get; set; }

    /// <summary>
    /// The version of the Slack client, if available.
    /// </summary>
    public string? SlackClientVersion { get; set; }

    /// <summary>
    /// The IP address for the session.
    /// </summary>
    public string? Ip { get; set; }
}
