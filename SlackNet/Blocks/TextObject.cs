namespace SlackNet.Blocks;

/// <summary>
/// An object containing some text, formatted either as plain text or using Slack's "mrkdwn".
/// </summary>
public abstract class TextObject
{
    protected TextObject(string type)
    {
        Type = type;
    }

    public string Type { get; set; }

    /// <summary>
    /// The text for the block.
    /// For <see cref="Markdown"/> text objects, this field accepts any of the standard text formatting markup.
    /// </summary>
    public string Text { get; set; }

    public static implicit operator TextObject(string text) => new PlainText(text);
}