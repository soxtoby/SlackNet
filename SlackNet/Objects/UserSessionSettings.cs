#nullable enable
using System;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// User-specific session settings, as returned by <see cref="WebApi.IAdminUsersSessionApi.GetSettings"/>.
/// </summary>
public class UserSessionSettings
{
    /// <summary>
    /// Encoded ID of the user.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Whether the session is terminated when the client—either the desktop app or a browser window—is closed.
    /// </summary>
    public bool? DesktopAppBrowserQuit { get; set; }

    /// <summary>
    /// The user's session duration, in seconds.
    /// </summary>
    [JsonProperty("duration")]
    public int? DurationSeconds { get; set; }

    [JsonIgnore]
    public TimeSpan? Duration => DurationSeconds is { } seconds ? TimeSpan.FromSeconds(seconds) : null;
}
