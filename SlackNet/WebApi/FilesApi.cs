using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IFilesApi
    {
        /// <summary>
        /// Deletes a file from your team.
        /// </summary>
        /// <param name="fileId">ID of file to delete.</param>
        /// <param name="cancellationToken"></param>
        Task Delete(string fileId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns information about a file in your team.
        /// </summary>
        /// <param name="fileId">Specify a file by providing its ID.</param>
        /// <param name="count">Number of comments to return per page.</param>
        /// <param name="page">Page number of comments to return.</param>
        /// <param name="cursor">
        /// Parameter for pagination. File comments are paginated for a single file.
        /// Set cursor equal to the <see cref="ResponseMetadata.NextCursor"/> returned by the previous request's <see cref="FileAndCommentsResponse.ResponseMetadata"/>.
        /// </param>
        /// <param name="cancellationToken"></param>
        Task<FileAndCommentsResponse> Info(string fileId, int count = 100, int page = 1, string cursor = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns a list of files within the team. It can be filtered and sliced in various ways.
        /// </summary>
        /// <param name="userId">Filter files created by a single user.</param>
        /// <param name="channelId">Filter files appearing in a specific channel, indicated by its ID.</param>
        /// <param name="tsFrom">Filter files created after this timestamp (inclusive).</param>
        /// <param name="tsTo">Filter files created before this timestamp (inclusive).</param>
        /// <param name="types">Filter files by type.</param>
        /// <param name="count">Number of items to return per page.</param>
        /// <param name="page">Page number of results to return.</param>
        /// <param name="cancellationToken"></param>
        Task<FileListResponse> List(
            string userId = null,
            string channelId = null,
            string tsFrom = null,
            string tsTo = null,
            IEnumerable<FileType> types = null,
            int count = 100,
            int page = 1,
            CancellationToken? cancellationToken = null
        );

        /// <summary>
        /// Disables public/external sharing for a file.
        /// </summary>
        /// <param name="fileId">File to revoke</param>
        /// <param name="cancellationToken"></param>
        Task<FileResponse> RevokePublicUrl(string fileId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Enables public/external sharing for a file.
        /// </summary>
        /// <param name="fileId">File to share.</param>
        /// <param name="cancellationToken"></param>
        Task<FileAndCommentsResponse> SharedPublicUrl(string fileId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Allows you to create or upload an existing file.
        /// </summary>
        /// <param name="fileContents">Contents of text file.</param>
        /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
        /// <param name="fileName">Filename of file.</param>
        /// <param name="title">Title of file</param>
        /// <param name="initialComment">Initial comment to add to file.</param>
        /// <param name="threadTs">Provide another message's <see cref="MessageEvent.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
        /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
        /// <param name="cancellationToken"></param>
        Task<FileResponse> Upload(
            string fileContents,
            string fileType = null,
            string fileName = null,
            string title = null,
            string initialComment = null,
            string threadTs = null,
            IEnumerable<string> channels = null,
            CancellationToken? cancellationToken = null
        );

        /// <summary>
        /// Allows you to create or upload an existing file.
        /// </summary>
        /// <param name="fileContents">Contents of file.</param>
        /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
        /// <param name="fileName">Filename of file.</param>
        /// <param name="title">Title of file</param>
        /// <param name="initialComment">Initial comment to add to file.</param>
        /// <param name="threadTs">Provide another message's <see cref="MessageEvent.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
        /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
        /// <param name="cancellationToken"></param>
        Task<FileResponse> Upload(byte[] fileContents,
            string fileType = null,
            string fileName = null,
            string title = null,
            string initialComment = null,
            string threadTs = null,
            IEnumerable<string> channels = null,
            CancellationToken? cancellationToken = null
        );

        /// <summary>
        /// Allows you to create or upload an existing file.
        /// </summary>
        /// <param name="fileContents">Contents of file.</param>
        /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
        /// <param name="fileName">Filename of file.</param>
        /// <param name="title">Title of file</param>
        /// <param name="initialComment">Initial comment to add to file.</param>
        /// <param name="threadTs">Provide another message's <see cref="MessageEvent.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
        /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
        /// <param name="cancellationToken"></param>
        Task<FileResponse> Upload(
            Stream fileContents,
            string fileType = null,
            string fileName = null,
            string title = null,
            string initialComment = null,
            string threadTs = null,
            IEnumerable<string> channels = null,
            CancellationToken? cancellationToken = null
        );

        /// <summary>
        /// Allows you to create or upload an existing file as a snippet.
        /// Used for creating a "file" from a long message/paste and forces "editable" mode.
        /// There is a 1 megabyte file size limit for files uploaded as snippets.
        /// </summary>
        /// <param name="snippet">Contents file text file.</param>
        /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
        /// <param name="fileName">Filename of file.</param>
        /// <param name="title">Title of file</param>
        /// <param name="initialComment">Initial comment to add to file.</param>
        /// <param name="threadTs">Provide another message's <see cref="MessageEvent.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
        /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
        /// <param name="cancellationToken"></param>
        Task<FileResponse> UploadSnippet(
            string snippet,
            string fileType = null,
            string fileName = null,
            string title = null,
            string initialComment = null,
            string threadTs = null,
            IEnumerable<string> channels = null,
            CancellationToken? cancellationToken = null
        );
    }

    public class FilesApi : IFilesApi
    {
        private readonly ISlackApiClient _client;
        public FilesApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Deletes a file from your team.
        /// </summary>
        /// <param name="fileId">ID of file to delete.</param>
        /// <param name="cancellationToken"></param>
        public Task Delete(string fileId, CancellationToken? cancellationToken = null) =>
            _client.Post("files.delete", new Args { { "file", fileId } }, cancellationToken);

        /// <summary>
        /// Returns information about a file in your team.
        /// </summary>
        /// <param name="fileId">Specify a file by providing its ID.</param>
        /// <param name="count">Number of comments to return per page.</param>
        /// <param name="page">Page number of comments to return.</param>
        /// <param name="cancellationToken"></param>
        public Task<FileAndCommentsResponse> Info(string fileId, int count = 100, int page = 1, string cursor = null, CancellationToken? cancellationToken = null) =>
            _client.Get<FileAndCommentsResponse>("files.info", new Args
                {
                    { "file", fileId },
                    { "count", count },
                    { "page", page },
                    { "cursor", cursor }
                }, cancellationToken);

        /// <summary>
        /// Returns a list of files within the team. It can be filtered and sliced in various ways.
        /// </summary>
        /// <param name="userId">Filter files created by a single user.</param>
        /// <param name="channelId">Filter files appearing in a specific channel, indicated by its ID.</param>
        /// <param name="tsFrom">Filter files created after this timestamp (inclusive).</param>
        /// <param name="tsTo">Filter files created before this timestamp (inclusive).</param>
        /// <param name="types">Filter files by type.</param>
        /// <param name="count">Number of items to return per page.</param>
        /// <param name="page">Page number of results to return.</param>
        /// <param name="cancellationToken"></param>
        public Task<FileListResponse> List(
            string userId = null,
            string channelId = null,
            string tsFrom = null,
            string tsTo = null,
            IEnumerable<FileType> types = null,
            int count = 100,
            int page = 1,
            CancellationToken? cancellationToken = null
        ) =>
            _client.Get<FileListResponse>("files.list", new Args
                {
                    { "user", userId },
                    { "channel", channelId },
                    { "ts_from", tsFrom },
                    { "ts_to", tsTo },
                    { "types", types },
                    { "count", count },
                    { "page", page }
                }, cancellationToken);

        /// <summary>
        /// Disables public/external sharing for a file.
        /// </summary>
        /// <param name="fileId">File to revoke</param>
        /// <param name="cancellationToken"></param>
        public Task<FileResponse> RevokePublicUrl(string fileId, CancellationToken? cancellationToken = null) =>
            _client.Post<FileResponse>("files.revokePublicURL", new Args { { "file", fileId } }, cancellationToken);

        /// <summary>
        /// Enables public/external sharing for a file.
        /// </summary>
        /// <param name="fileId">File to share.</param>
        /// <param name="cancellationToken"></param>
        public Task<FileAndCommentsResponse> SharedPublicUrl(string fileId, CancellationToken? cancellationToken = null) =>
            _client.Post<FileAndCommentsResponse>("files.sharedPublicURL", new Args { { "file", fileId } }, cancellationToken);

        /// <summary>
        /// Allows you to create or upload an existing file.
        /// </summary>
        /// <param name="fileContents">Contents of text file.</param>
        /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
        /// <param name="fileName">Filename of file.</param>
        /// <param name="title">Title of file</param>
        /// <param name="initialComment">Initial comment to add to file.</param>
        /// <param name="threadTs">Provide another message's <see cref="MessageEvent.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
        /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
        /// <param name="cancellationToken"></param>
        public Task<FileResponse> Upload(
            string fileContents,
            string fileType = null,
            string fileName = null,
            string title = null,
            string initialComment = null,
            string threadTs = null,
            IEnumerable<string> channels = null,
            CancellationToken? cancellationToken = null
        ) =>
            Upload(new StringContent(fileContents), fileType, fileName, title, initialComment, threadTs, channels, cancellationToken);

        /// <summary>
        /// Allows you to create or upload an existing file.
        /// </summary>
        /// <param name="fileContents">Contents of file.</param>
        /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
        /// <param name="fileName">Filename of file.</param>
        /// <param name="title">Title of file</param>
        /// <param name="initialComment">Initial comment to add to file.</param>
        /// <param name="threadTs">Provide another message's <see cref="MessageEvent.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
        /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
        /// <param name="cancellationToken"></param>
        public Task<FileResponse> Upload(
            byte[] fileContents,
            string fileType = null,
            string fileName = null,
            string title = null,
            string initialComment = null,
            string threadTs = null,
            IEnumerable<string> channels = null,
            CancellationToken? cancellationToken = null
        ) =>
            Upload(new ByteArrayContent(fileContents), fileType, fileName, title, initialComment, threadTs, channels, cancellationToken);

        /// <summary>
        /// Allows you to create or upload an existing file.
        /// </summary>
        /// <param name="fileContents">Contents of file.</param>
        /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
        /// <param name="fileName">Filename of file.</param>
        /// <param name="title">Title of file</param>
        /// <param name="initialComment">Initial comment to add to file.</param>
        /// <param name="threadTs">Provide another message's <see cref="MessageEvent.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
        /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
        /// <param name="cancellationToken"></param>
        public Task<FileResponse> Upload(
            Stream fileContents,
            string fileType = null,
            string fileName = null,
            string title = null,
            string initialComment = null,
            string threadTs = null,
            IEnumerable<string> channels = null,
            CancellationToken? cancellationToken = null
        ) =>
            Upload(new StreamContent(fileContents), fileType, fileName, title, initialComment, threadTs, channels, cancellationToken);

        private Task<FileResponse> Upload(
            HttpContent fileContent,
            string fileType,
            string fileName,
            string title,
            string initialComment,
            string threadTs,
            IEnumerable<string> channels,
            CancellationToken? cancellationToken
        ) =>
            _client.Post<FileResponse>("files.upload", new Args
                    {
                        { "filetype", fileType },
                        { "filename", fileName },
                        { "title", title },
                        { "initial_comment", initialComment },
                        { "channels", channels },
                        { "thread_ts", threadTs }
                    },
                new MultipartFormDataContent { { fileContent, "file", fileName ?? "file" } },
                cancellationToken);

        /// <summary>
        /// Allows you to create or upload an existing file as a snippet.
        /// Used for creating a "file" from a long message/paste and forces "editable" mode.
        /// There is a 1 megabyte file size limit for files uploaded as snippets.
        /// </summary>
        /// <param name="snippet">Contents file text file.</param>
        /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
        /// <param name="fileName">Filename of file.</param>
        /// <param name="title">Title of file</param>
        /// <param name="initialComment">Initial comment to add to file.</param>
        /// <param name="threadTs">Provide another message's <see cref="MessageEvent.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
        /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
        /// <param name="cancellationToken"></param>
        public Task<FileResponse> UploadSnippet(
            string snippet,
            string fileType = null,
            string fileName = null,
            string title = null,
            string initialComment = null,
            string threadTs = null,
            IEnumerable<string> channels = null,
            CancellationToken? cancellationToken = null
        ) =>
            _client.Post<FileResponse>("files.upload", new Args(),
                new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "filetype", fileType },
                        { "filename", fileName },
                        { "title", title },
                        { "initial_comment", initialComment },
                        { "channels", string.Join(",", channels) },
                        { "thread_ts", threadTs },
                        { "content", snippet }
                    }),
                cancellationToken);
    }
}
