namespace SlackNet.Blocks;

public interface IBlockElement
{
    string Type { get; }
}

public abstract class BlockElement : IBlockElement
{
    protected BlockElement(string type) => Type = type;

    public string Type { get; }
}