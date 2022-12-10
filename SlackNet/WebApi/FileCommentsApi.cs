using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IFileCommentsApi
{
    /// <summary>
    /// Delete an existing comment on a file.
    /// Only the original author of the comment or a Team Administrator may delete a file comment.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.comments.delete">Slack documentation</a> for more information.</remarks>
    /// <param name="fileId">File to delete a comment from.</param>
    /// <param name="commentId">The comment to delete.</param>
    /// <param name="cancellationToken"></param>
    Task Delete(string fileId, string commentId, CancellationToken? cancellationToken = null);
}

public class FileCommentsApi : IFileCommentsApi
{
    private readonly ISlackApiClient _client;
    public FileCommentsApi(ISlackApiClient client) => _client = client;

    public Task Delete(string fileId, string commentId, CancellationToken? cancellationToken = null) =>
        _client.Post("files.comments.delete", new Args
            {
                { "file", fileId },
                { "id", commentId }
            }, cancellationToken);
}