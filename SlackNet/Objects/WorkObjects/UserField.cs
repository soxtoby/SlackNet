namespace SlackNet;

[SlackType("slack#/types/user")]
public class UserField() : EntityField("slack#/types/user")
{
    public EntityUser User { get; set; } = new();
}