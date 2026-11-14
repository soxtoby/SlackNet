#nullable enable
namespace SlackNet;

/// <summary>
/// Info on an app.
/// The <see cref="HelpUrl"/>, <see cref="PrivacyPolicyUrl"/>, <see cref="AppHomepageUrl"/>, and <see cref="AppDirectoryUrl"/> properties contain links to more info.
/// </summary>
public class App
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? HelpUrl { get; set; }
    public string? PrivacyPolicyUrl { get; set; }
    public string? AppHomepageUrl { get; set; }
    public string? AppDirectoryUrl { get; set; }
    public bool IsGranularBotApp { get; set; }
    public bool IsAppDirectoryApproved { get; set; }
    public bool IsInternal { get; set; }
    public string? AdditionalInfo { get; set; }

    /// <summary>
    /// Describes the app's origin.
    /// </summary>
    public AppDeveloperType? DeveloperType { get; set; }

    public bool SocketModeEnabled { get; set; }
    public Icons? Icons { get; set; }
}

/// <summary>
/// The origin of an <see cref="App"/>.
/// </summary>
public enum AppDeveloperType
{
    /// <summary>
    /// The app was developed as part of this Enterprise organization or workspace.
    /// </summary>
    Internal,

    /// <summary>
    /// The app was developed by a third party, such as (but not limited to) those found in the Slack Marketplace.
    /// </summary>
    ThirdParty,

    /// <summary>
    /// The app was built with love by Slack.
    /// </summary>
    Slack
}
