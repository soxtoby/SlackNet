using System.Runtime.Serialization;

namespace SlackNet.Objects
{
    public enum ChannelType
    {
        [EnumMember(Value = "C")] Channel,
        [EnumMember(Value = "G")] PrivateGroup
    }
}