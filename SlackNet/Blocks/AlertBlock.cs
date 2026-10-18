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

/// <summary>Controls the visual emphasis applied to an <see cref="AlertBlock"/>.</summary>
public enum AlertLevel
{
    /// <summary>Uses Slack's default alert styling.</summary>
    Default,
    /// <summary>Indicates informational content.</summary>
    Info,
    /// <summary>Calls attention to a potential problem.</summary>
    Warning,
    /// <summary>Indicates an error or failed operation.</summary>
    Error,
    /// <summary>Indicates a successful operation.</summary>
    Success
}
