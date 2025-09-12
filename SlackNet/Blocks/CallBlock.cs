namespace SlackNet.Blocks;

/// <summary>
/// Shows information about a call.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/apis/web-api/using-the-calls-api/#post_to_channel">Slack documentation</a> for more information.</remarks>
[SlackType("call")]
public class CallBlock : Block
{
    public CallBlock() : base("call") { }

    public string CallId { get; set; }
}