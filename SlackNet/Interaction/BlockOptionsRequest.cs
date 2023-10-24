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
}