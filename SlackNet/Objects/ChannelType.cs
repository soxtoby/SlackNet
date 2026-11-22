using System.Runtime.Serialization;

namespace SlackNet;

public enum ChannelType
{
    [EnumMember(Value = "C")] Channel,
    [EnumMember(Value = "G")] PrivateGroup
}