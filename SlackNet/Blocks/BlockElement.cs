namespace SlackNet.Blocks;

public interface IBlockElement
{
    string Type { get; }
}

public abstract class BlockElement(string type) : SlackObject, IBlockElement
{
    public string Type { get; } = type;
}