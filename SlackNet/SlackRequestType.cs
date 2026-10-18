namespace SlackNet;

/// <summary>
/// Identifies the kind of request received from Slack.
/// </summary>
public enum SlackRequestType
{
    /// <summary>An Events API event.</summary>
    Event,
    /// <summary>An Events API URL-verification challenge.</summary>
    UrlVerification,
    /// <summary>An interactive component action.</summary>
    Action,
    /// <summary>An external data-source options request.</summary>
    Options,
    /// <summary>A slash-command invocation.</summary>
    SlashCommand
}
