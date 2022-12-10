namespace SlackNet.Blocks;

/// <summary>
/// An object that defines a dialog that provides a confirmation step to any interactive element.
/// This dialog will ask the user to confirm their action by offering a confirm and deny buttons.
/// </summary>
public class ConfirmationDialog
{
    /// <summary>
    /// A plain text object that defines the dialog's title.
    /// </summary>
    public PlainText Title { get; set; }

    /// <summary>
    /// A text object that defines the explanatory text that appears in the confirm dialog.
    /// </summary>
    public TextObject Text { get; set; }

    /// <summary>
    /// A plain text object to define the text of the button that confirms the action.
    /// </summary>
    public PlainText Confirm { get; set; }

    /// <summary>
    /// A plain text object to define the text of the button that cancels the action.
    /// </summary>
    public PlainText Deny { get; set; }

    /// <summary>
    /// Defines the color scheme applied to the confirm button. A value of <c>danger</c> will display the button with a red background on desktop, or red text on mobile.
    /// A value of <c>primary</c> will display the button with a green background on desktop, or blue text on mobile.
    /// If this field is not provided, the default value will be <c>primary</c>.
    /// </summary>
    public string Style { get; set; }
}