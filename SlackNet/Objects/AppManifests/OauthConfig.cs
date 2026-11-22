using System.Collections.Generic;

namespace SlackNet.AppManifests;

/// <summary>
/// A group of settings describing OAuth configuration for the app.
/// </summary>
public class OauthConfig
{
    /// <summary>
    /// Whether or not the app supports <a href="https://docs.slack.dev/authentication/enterprise-managed-authorization/">enterprise-managed authorization</a>.
    /// </summary>
    public bool? IsEnterpriseManagedAuthSupported { get; set; }

    /// <summary>
    /// Whether or not <a href="https://oauth.net/2/pkce/">PKCE</a> is enabled for the OAuth flow.
    /// </summary>
    public bool? PkceEnabled { get; set; }

    /// <summary>
    /// <a href="https://docs.slack.dev/authentication/installing-with-oauth/#asking">OAuth redirect URLs</a>.
    /// </summary>
    public IList<string>? RedirectUrls { get; set; }

    /// <summary>
    /// Settings that describe <a href="https://docs.slack.dev/reference/scopes/">permission scopes</a> configuration.
    /// </summary>
    public Scopes? Scopes { get; set; }

    /// <summary>
    /// Whether token management should be enabled.
    /// </summary>
    public bool? TokenManagementEnabled { get; set; }
}

/// <summary>
/// Settings that describe <a href="https://docs.slack.dev/reference/scopes/">permission scopes</a> configuration.
/// </summary>
public class Scopes
{
    /// <summary>
    /// Bot scopes to request upon app installation.
    /// </summary>
    public IList<string>? Bot { get; set; }

    /// <summary>
    /// <a href="https://docs.slack.dev/authentication/installing-with-oauth/#optional-scopes">Optional bot scopes</a>.
    /// Optional scopes must also be listed in <see cref="Bot"/>.
    /// </summary>
    public IList<string>? BotOptional { get; set; }

    /// <summary>
    /// User scopes to request upon app installation.
    /// </summary>
    public IList<string>? User { get; set; }

    /// <summary>
    /// <a href="https://docs.slack.dev/authentication/installing-with-oauth/#optional-scopes">Optional user scopes</a>.
    /// Optional scopes must also be listed in <see cref="User"/>.
    /// </summary>
    public IList<string>? UserOptional { get; set; }
}
