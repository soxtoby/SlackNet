#nullable enable
namespace SlackNet.AppManifests;

/// <summary>
/// Settings that describe <a href="https://docs.slack.dev/surfaces/app-home/">App Home</a> configuration.
/// </summary>
public class AppHome
{
    /// <summary>
    /// Whether or not the <a href="https://docs.slack.dev/surfaces/app-home/#home-tab">Home tab</a> is enabled.
    /// </summary>
    public bool? HomeTabEnabled { get; set; }

    /// <summary>
    /// Whether or not the <a href="https://docs.slack.dev/surfaces/app-home/#messages-tab">Messages tab in your App Home</a> is enabled.
    /// </summary>
    public bool? MessagesTabEnabled { get; set; }

    /// <summary>
    /// Whether or not the users can send messages to your app in the Messages tab of your App Home.
    /// </summary>
    public bool? MessagesTabReadOnlyEnabled { get; set; }
}
