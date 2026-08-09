namespace SlackNet.Blocks;

/// <summary>
/// Displays an alert message in a modal.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/alert-block">Slack documentation</a> for more information.</remarks>
[SlackType("alert")]
public class AlertBlock() : Block("alert")
{
    /// <summary>
    /// The alert text.
    /// </summary>
    public TextObject Text { get; set; }

    /// <summary>
    /// The visual style of the alert.
    /// </summary>
    [IgnoreIfDefault]
    public AlertLevel Level { get; set; }
}

public enum AlertLevel
{
    Default,
    Info,
    Warning,
    Error,
    Success
}
