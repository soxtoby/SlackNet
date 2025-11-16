namespace SlackNet;

[SlackType("integer")]
public class IntegerField() : EntityField("integer")
{
    public int Value { get; set; }
}