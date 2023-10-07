namespace SlackNet.Blocks;

public class EmailTextInput : TextInput
{
    public EmailTextInput() : base("email_text_input") { }

    /// <summary>
    /// The initial value in the input when it is loaded.
    /// </summary>
    public string InitialValue { get; set; }
}

[SlackType("email_text_input")]
public class EmailTextInputAction : BlockAction
{
    public string Value { get; set; }
}

[SlackType("email_text_input")]
public class EmailTextInputValue : ElementValue
{
    public string Value { get; set; }
}