namespace SlackNet.Blocks;

public abstract class Block(string type)
{
    public string Type { get; } = type;

    /// <summary>
    /// A string acting as a unique identifier for a block. You can use this when you receive an interaction payload
    /// to identify the source of the action. If not specified, one will be generated.
    /// Should be unique for each message and each iteration of a message. If a message is updated, use a new block ID.
    /// </summary>
    public string BlockId { get; set; }
}