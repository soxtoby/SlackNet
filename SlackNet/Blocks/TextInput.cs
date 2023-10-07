using SlackNet.Interaction;

namespace SlackNet.Blocks;

public abstract class TextInput : BlockElement, IInputBlockElement
{
    protected TextInput(string type) : base(type) { }

    /// <summary>
    /// An identifier for the input value when the parent modal is submitted.
    /// You can use this when you receive a <see cref="ViewSubmission"/> payload to identify the value of the input element.
    /// Should be unique among all other <see cref="IActionElement.ActionId"/>s used elsewhere by your app.
    /// </summary>
    public string ActionId { get; set; }

    /// <summary>
    /// A plain text object that defines the placeholder text shown in the input. 
    /// </summary>
    public PlainText Placeholder { get; set; }

    /// <summary>
    /// A <see cref="DispatchActionConfig"/> that determines when during text input the element returns a <see cref="BlockAction"/> payload.
    /// </summary>
    public DispatchActionConfig DispatchActionConfig { get; set; }

    /// <summary>
    /// Indicates whether the element will be set to auto focus within the <see cref="ViewInfo"/> object. Only one element can be set to true.
    /// </summary>
    public bool FocusOnLoad { get; set; }
}