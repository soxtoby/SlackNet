namespace SlackNet.Blocks
{
    public abstract class BlockElement
    {
        protected BlockElement(string type) => Type = type;

        public string Type { get; }
    }
}