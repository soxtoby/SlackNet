namespace SlackNet.Blocks
{
    /// <summary>
    /// A non-interactive block element, like <see cref="ImageBlock"/> or <see cref="TextObject"/>.
    /// </summary>
    public interface IContextElement
    {
        string Type { get; }
    }
}