namespace SlackNet.Blocks;

public class UrlTextInput : TextInput
{
    public UrlTextInput() : base("url_text_input") { }
}

[SlackType("url_text_input")]
public class UrlTextInputAction : BlockAction
{
    public string Value { get; set; }
}

[SlackType("url_text_input")]
public class UrlTextInputValue : ElementValue
{
    public string Value { get; set; }
}