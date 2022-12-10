namespace SlackNet.Interaction;

public abstract class DialogElement
{
    protected DialogElement(string type) => Type = type;

    public string Type { get; set; }
    public string Label { get; set; }
    public string Name { get; set; }
    public bool Optional { get; set; }
    public string Value { get; set; }
    public string Placeholder { get; set; }
}