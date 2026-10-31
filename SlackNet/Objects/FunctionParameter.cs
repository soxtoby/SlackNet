namespace SlackNet;

/// <summary>
/// An input or output parameter defined by a function.
/// </summary>
public class FunctionParameter : SlackObject
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    public bool IsRequired { get; set; }
}
