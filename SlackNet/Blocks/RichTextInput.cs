namespace SlackNet.Blocks;

public class RichTextInput : TextInput
{
    public RichTextInput() : base("rich_text_input") { }

    /// <summary>
    /// The initial value in the rich text input when it is loaded.
    /// </summary>
    public RichTextBlock InitialValue { get; set; }
}

[SlackType("rich_text_input")]
public class RichTextInputAction : BlockAction
{
    public RichTextBlock RichTextValue { get; set; }
}

[SlackType("rich_text_input")]
public class RichTextInputValue : ElementValue
{
    public RichTextBlock RichTextValue { get; set; }
}