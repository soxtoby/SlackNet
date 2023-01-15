namespace SlackNet.Blocks;

public class PlainTextInput : TextInput
{
    public PlainTextInput() : base("plain_text_input") { }

    /// <summary>
    /// Indicates whether the input will be a single line (False) or a larger textarea (True). 
    /// </summary>
    public bool Multiline { get; set; }

    /// <summary>
    /// The minimum length of input that the user must provide. If the user provides less, they will receive an error.
    /// </summary>
    public int? MinLength { get; set; }

    /// <summary>
    /// The maximum length of input that the user can provide. If the user provides more, they will receive an error.
    /// </summary>
    public int? MaxLength { get; set; }
}

[SlackType("plain_text_input")]
public class PlainTextInputAction : BlockAction
{
    public string Value { get; set; }
}

[SlackType("plain_text_input")]
public class PlainTextInputValue : ElementValue
{
    public string Value { get; set; }
}