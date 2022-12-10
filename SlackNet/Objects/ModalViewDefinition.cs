using SlackNet.Blocks;
using SlackNet.Interaction;

namespace SlackNet;

public class ModalViewDefinition : ViewDefinition
{
    public ModalViewDefinition() : base("modal") { }

    /// <summary>
    /// The title that appears in the top-left of the modal.
    /// </summary>
    public PlainText Title { get; set; }

    /// <summary>
    /// Defines the text displayed in the close button at the bottom-right of the view.
    /// </summary>
    public PlainText Close { get; set; }

    /// <summary>
    /// Defines the text displayed in the submit button at the bottom-right of the view.
    /// Required when an input block is within the blocks array.
    /// </summary>
    public PlainText Submit { get; set; }

    /// <summary>
    /// When set to True, clicking on the close button will clear all views in a modal and close it.
    /// </summary>
    public bool ClearOnClose { get; set; }

    /// <summary>
    /// Indicates whether Slack will send your request URL a <see cref="ViewClosed"/> event when a user clicks the close button.
    /// </summary>
    public bool NotifyOnClose { get; set; }
}