namespace SlackNet.Blocks;

public interface IBlockElement
{
    string Type { get; }
}

public abstract class BlockElement(string type) : IBlockElement
{
    public string Type { get; } = type;
}