namespace SlackNet;

[SlackType("slack#/types/channel_id")]
public class ChannelIdField() : EntityField("slack#/types/channel_id")
{
    public string Value { get; set; }
}