#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SlackNet.WebApi;

namespace SlackNet;

/// <summary>
/// A request from a user on a team to install an app.
/// </summary>
public class AppRequest
{
    public required string Id { get; set; }

    /// <summary>
    /// Info on the app that the user has requested.
    /// </summary>
    public required App App { get; set; }

    /// <summary>
    /// Indicates whether the app was previously approved or restricted in this org.
    /// If the app installation has not been requested before, this will be null.
    /// </summary>
    public AppRequestResolution? PreviousResolution { get; set; }

    /// <summary>
    /// The user requesting the install.
    /// </summary>
    public required UserIdentity User { get; set; }

    /// <summary>
    /// The team that the app will be approved for. If null, then the approval request is for the whole enterprise.
    /// </summary>
    public Team? Team { get; set; }

    /// <summary>
    /// If not null, conveys that the approval is for whole enterprise.
    /// </summary>
    public WorkSpace? Enterprise { get; set; }

    /// <summary>
    /// The scopes that the app requests.
    /// </summary>
    public IList<AppScope> Scopes { get; set; } = [];

    /// <summary>
    /// An optional message from the app.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// App ID of the manager app if the requested app being installed was provisioned by one.
    /// </summary>
    public string? ManagerAppId { get; set; }

    /// <summary>
    /// Whether the installer is also a collaborator on the app.
    /// </summary>
    public bool IsUserAppCollaborator { get; set; }

    public int DateCreated { get; set; }
    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();
}

/// <summary>
/// Whether an app was previously approved or restricted.
/// </summary>
public class AppRequestResolution
{
    public AppResolutionStatus Status { get; set; }
    public IList<AppScope> Scopes { get; set; } = [];
}

public enum AppResolutionStatus
{
    Approved,
    Restricted
}
