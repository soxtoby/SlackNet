namespace SlackNet.Blocks;

/// <summary>
/// An object that represents a single selectable item in a select menu.
/// </summary>
public class Option
{
    /// <summary>
    /// A plain text object that defines the text shown in the option on the menu.
    /// </summary>
    public TextObject Text { get; set; }

    /// <summary>
    /// The string value that will be passed to your app when this option is chosen.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// A plain text object that defines a line of descriptive text shown below the text field beside a radio button.
    /// </summary>
    public PlainText Description { get; set; }
}