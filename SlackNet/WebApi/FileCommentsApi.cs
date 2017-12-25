using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IFileCommentsApi
    {
        /// <summary>
        /// Add a comment to an existing file.
        /// </summary>
        /// <param name="fileId">File to add a comment to.</param>
        /// <param name="comment">Text of the comment to add.</param>
        /// <param name="cancellationToken"></param>
        Task<FileComment> Add(string fileId, string comment, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Delete an existing comment on a file.
        /// Only the original author of the comment or a Team Administrator may delete a file comment.
        /// </summary>
        /// <param name="fileId">File to delete a comment from.</param>
        /// <param name="commentId">The comment to delete.</param>
        /// <param name="cancellationToken"></param>
        Task Delete(string fileId, string commentId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Edit an existing comment on a file. 
        /// Only the user who created a comment may make edits. 
        /// Teams may configure a limited time window during which file comment edits are allowed.
        /// </summary>
        /// <param name="fileId">File containing the comment to edit.</param>
        /// <param name="commentId">The comment to edit.</param>
        /// <param name="comment">Text of the comment to edit.</param>
        /// <param name="cancellationToken"></param>
        Task<FileComment> Edit(string fileId, string commentId, string comment, CancellationToken? cancellationToken = null);
    }

    public class FileCommentsApi : IFileCommentsApi
    {
        private readonly ISlackApiClient _client;
        public FileCommentsApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Add a comment to an existing file.
        /// </summary>
        /// <param name="fileId">File to add a comment to.</param>
        /// <param name="comment">Text of the comment to add.</param>
        /// <param name="cancellationToken"></param>
        public async Task<FileComment> Add(string fileId, string comment, CancellationToken? cancellationToken = null) =>
            (await _client.Get<FileCommentResponse>("file.comments.add", new Args
                {
                    { "file", fileId },
                    { "comment", comment }
                }, cancellationToken).ConfigureAwait(false))
            .Comment;

        /// <summary>
        /// Delete an existing comment on a file.
        /// Only the original author of the comment or a Team Administrator may delete a file comment.
        /// </summary>
        /// <param name="fileId">File to delete a comment from.</param>
        /// <param name="commentId">The comment to delete.</param>
        /// <param name="cancellationToken"></param>
        public Task Delete(string fileId, string commentId, CancellationToken? cancellationToken = null) =>
            _client.Get("file.comments.delete", new Args
                {
                    { "file", fileId },
                    { "id", commentId }
                }, cancellationToken);

        /// <summary>
        /// Edit an existing comment on a file. 
        /// Only the user who created a comment may make edits. 
        /// Teams may configure a limited time window during which file comment edits are allowed.
        /// </summary>
        /// <param name="fileId">File containing the comment to edit.</param>
        /// <param name="commentId">The comment to edit.</param>
        /// <param name="comment">Text of the comment to edit.</param>
        /// <param name="cancellationToken"></param>
        public async Task<FileComment> Edit(string fileId, string commentId, string comment, CancellationToken? cancellationToken = null) =>
            (await _client.Get<FileCommentResponse>("files.comments.edit", new Args
                {
                    { "file", fileId },
                    { "id", commentId },
                    { "comment", comment }
                }, cancellationToken).ConfigureAwait(false))
            .Comment;
    }
}