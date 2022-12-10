namespace SlackNet.Interaction;

public abstract class ActionElement
{
    protected ActionElement(string type) => Type = type;

    public string Type { get; }
    public string Name { get; set; }
    public string Text { get; set; }
    public string Value { get; set; }
    public Confirm Confirm { get; set; }
}