using System.Runtime.Serialization;

namespace SlackNet.Interaction
{
    public enum DialogElementType
    {
        Text,
        [EnumMember(Value = "textarea")] TextArea,
        Select
    }
}