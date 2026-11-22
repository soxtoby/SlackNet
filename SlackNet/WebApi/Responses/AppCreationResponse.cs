namespace SlackNet.WebApi;

public class AppCreationResponse
{
    public required string AppId { get; set; }
    public AppCredentials Credentials { get; set; } = new();
    public string? OauthAuthorizeUrl { get; set; }

    /// <summary>
    /// The team ID where the app was created.
    /// </summary>
    public string? TeamId { get; set; }

    /// <summary>
    /// The team domain where the app was created.
    /// </summary>
    public string? TeamDomain { get; set; }

    public string? AppToken { get; set; }
}
