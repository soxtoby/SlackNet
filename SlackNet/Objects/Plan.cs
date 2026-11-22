using System.Runtime.Serialization;

namespace SlackNet;

public enum Plan
{
    Free,
    [EnumMember(Value = "std")] Pro,
    [EnumMember(Value = "plus")] BusinessPlus,
    [EnumMember(Value = "enterprise")] EnterpriseGrid,
    [EnumMember(Value = "compliance")] EnterpriseComplianceSelect
}