namespace SlackNet;

public abstract class EntityField()
{
    protected EntityField(string type) : this() => Type = type;

    public string Type { get; set; }

    public string Key { get; set; }
    public string Label { get; set; }

    public EntityEditSupport Edit { get; set; } = new();
}