namespace SlackNet.Blocks;

/// <summary>
/// An interactive element that inserts a button. The button can be a trigger for anything from opening a simple link to starting a complex workflow.
/// </summary>
[SlackType("button")]
public class Button() : ActionElement("button")
{
    /// <summary>
    /// A text object that defines the button's text.
    /// </summary>
    public PlainText Text { get; set; }

    /// <summary>
    /// A URL to load in the user's browser when the button is clicked.
    /// If you're using <see cref="Url"/>, you'll still receive an interaction payload and will need to send an acknowledgement response.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// The value to send along with the interaction payload. 
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Decorates buttons with alternative visual color schemes. Use this option with restraint.
    /// </summary>
    [IgnoreIfDefault]
    public ButtonStyle Style { get; set; }

    /// <summary>
    /// A label for longer descriptive text about a button element. This label will be read out by screen readers instead of the button <see cref="Text"/> object.
    /// </summary>
    public string AccessibilityLabel { get; set; }
}

public enum ButtonStyle
{
    /// <summary>
    /// The default style. Most buttons should have this style.
    /// </summary>
    Default,

    /// <summary>
    /// Gives buttons a green outline and text, ideal for affirmation or confirmation actions. Should only be used for one button within a set.
    /// </summary>
    Primary,

    /// <summary>
    /// Gives buttons a red outline and text, and should be used when the action is destructive. Use even more sparingly than <see cref="Primary"/>.
    /// </summary>
    Danger
}

[SlackType("button")]
public class ButtonAction : BlockAction
{
    public TextObject Text { get; set; }
    public string Value { get; set; }
}