namespace SlackNet.WebApi;

public enum AccessLevel
{
    Read,
    Write,
    Owner,

    /// <summary>
    /// Read and comment, but not edit. Used by <see cref="IAgentsConversationsApi.SetCanvasView"/>.
    /// </summary>
    Comment
}