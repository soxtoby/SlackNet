using System.Runtime.Serialization;

namespace SlackNet;

public enum PinnedItemType
{
    [EnumMember(Value = "C")] ChannelMessage,
    [EnumMember(Value = "G")] PrivateGroupMessage,
    [EnumMember(Value = "F")] File,
    [EnumMember(Value = "Fc")] FileComment
}