using SlackNet.Interaction;

namespace SlackNet.Blocks;

public class PlainTextInput : BlockElement, IInputBlockElement
{
    public PlainTextInput() : base("plain_text_input") { }

    /// <summary>
    /// An identifier for the input value when the parent modal is submitted.
    /// You can use this when you receive a <see cref="ViewSubmission"/> payload to identify the value of the input element.
    /// Should be unique among all other <see cref="IActionElement.ActionId"/>s used elsewhere by your app.
    /// </summary>
    public string ActionId { get; set; }

    /// <summary>
    /// A plain text object that defines the placeholder text shown in the plain-text input. 
    /// </summary>
    public PlainText Placeholder { get; set; }

    /// <summary>
    /// The initial value in the plain-text input when it is loaded.
    /// </summary>
    public string InitialValue { get; set; }

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
        
    /// <summary>
    /// A <see cref="DispatchActionConfig"/> that determines when during text input the element returns a <see cref="BlockAction"/> payload.
    /// </summary>
    public DispatchActionConfig DispatchActionConfig { get; set; }

    /// <summary>
    /// Indicates whether the element will be set to auto focus within the <see cref="ViewInfo"/> object. Only one element can be set to true.
    /// </summary>
    public bool FocusOnLoad { get; set; }
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