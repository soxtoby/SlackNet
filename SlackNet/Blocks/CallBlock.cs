namespace SlackNet.Blocks;

[SlackType("call")]
public class CallBlock : Block
{
    public CallBlock() : base("call") { }

    public string CallId { get; set; }
}