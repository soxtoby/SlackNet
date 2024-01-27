namespace SlackNet.Blocks;

public interface IActionElement : IBlockElement
{
    string ActionId { get; set; }
}

public abstract class ActionElement(string type) : BlockElement(type), IActionElement
{
    /// <summary>
    /// An identifier for this action. You can use this when you receive an interaction payload to identify the source of the action.
    /// Should be unique among all other <see cref="ActionId"/>s used elsewhere by your app. 
    /// </summary>
    public string ActionId { get; set; }

    /// <summary>
    /// Defines an optional confirmation dialog that appears after the element is activated.
    /// </summary>
    public ConfirmationDialog Confirm { get; set; }
}