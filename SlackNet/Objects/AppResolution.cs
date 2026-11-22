using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// An app that has been approved or restricted for installation on a workspace or Enterprise organization.
/// </summary>
public class AppResolution
{
    public required App App { get; set; }

    /// <summary>
    /// The scopes that were approved or restricted.
    /// </summary>
    public IList<AppScope> Scopes { get; set; } = [];

    public int DateUpdated { get; set; }

    [JsonIgnore]
    public DateTime Updated => DateUpdated.ToDateTime().GetValueOrDefault();

    /// <summary>
    /// Who last approved or restricted the app.
    /// </summary>
    public Actor? LastResolvedBy { get; set; }
}
