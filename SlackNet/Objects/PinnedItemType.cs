using System.Runtime.Serialization;

namespace SlackNet.Objects
{
    public enum PinnedItemType
    {
        [EnumMember(Value = "C")] ChannelMessage,
        [EnumMember(Value = "G")] PrivateGroupMessage,
        [EnumMember(Value = "F")] File,
        [EnumMember(Value = "Fc")] FileComment
    }
}