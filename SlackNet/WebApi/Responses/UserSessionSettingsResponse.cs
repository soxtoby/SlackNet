#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class UserSessionSettingsResponse
{
    /// <summary>
    /// User-specific session settings.
    /// </summary>
    public IList<UserSessionSettings> SessionSettings { get; set; } = [];

    /// <summary>
    /// The IDs of users requested that do not have any user session settings applied.
    /// </summary>
    public IList<string> NoSettingsApplied { get; set; } = [];
}
