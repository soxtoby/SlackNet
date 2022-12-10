namespace SlackNet.Blocks;

/// <summary>
/// A block that collects information from users.
/// </summary>
[SlackType("input")] 
public class InputBlock : Block
{
    public InputBlock() : base("input") { }

    /// <summary>
    /// A label that appears above an input element.
    /// </summary>
    public PlainText Label { get; set; }

    /// <summary>
    /// The input element to appear in the block.
    /// </summary>
    public IInputBlockElement Element { get; set; }

    /// <summary>
    /// An optional hint that appears below an input element in a lighter grey.
    /// </summary>
    public PlainText Hint { get; set; }

    /// <summary>
    /// A boolean that indicates whether the input element may be empty when a user submits the modal.
    /// </summary>
    public bool Optional { get; set; }
        
    /// <summary>
    /// A boolean that indicates whether or not the use of elements in this block should dispatch a <see cref="BlockAction"/> payload. Defaults to false.
    /// </summary>
    public bool DispatchAction { get; set; }
}