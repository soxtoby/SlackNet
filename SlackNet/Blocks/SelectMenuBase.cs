namespace SlackNet.Blocks;

public abstract class SelectMenuBase : ActionElement, IInputBlockElement
{
    protected SelectMenuBase(string type) : base(type) { }

    /// <summary>
    /// A plain text object that defines the placeholder text shown on the menu. 
    /// </summary>
    public PlainText Placeholder { get; set; }

    /// <summary>
    /// Indicates whether the element will be set to auto focus within the <see cref="ViewInfo"/> object. Only one element can be set to true.
    /// </summary>
    [IgnoreIfDefault] // See issue #177 - Slack returns an invalid parameter for ExternalMultiSelectMenu
    public bool FocusOnLoad { get; set; }
}