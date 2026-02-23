using SlackNet.Events;

namespace SlackNet.Interaction;

[SlackType("block_suggestion")]
public class BlockOptionsRequest : OptionsRequestBase
{
    public Container Container { get; set; }
    public string ApiAppId { get; set; }
    public string ActionId { get; set; }
    public string BlockId { get; set; }
    public MessageEvent Message { get; set; }
    public ViewInfo View { get; set; }

    /// <summary>
    /// Metadata about the function execution that generated the block where this block suggestion took place.
    /// </summary>
    public FunctionData FunctionData { get; set; }

    /// <summary>
    /// A workflow (just-in-time) token generated for this block suggestion.
    /// </summary>
    public string BotAccessToken { get; set; }
}
