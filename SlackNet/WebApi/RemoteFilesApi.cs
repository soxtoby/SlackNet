using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IRemoteFilesApi
{
    /// <summary>
    /// Adds a file from a remote service.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.add">Slack documentation</a> for more information.</remarks>
    /// <param name="externalId">Creator defined unique ID for the file.</param>
    /// <param name="externalUrl">URL of the remote file.</param>
    /// <param name="title">Title of the file being shared.</param>
    /// <param name="filetype">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="indexableFileContents">Contents that can be used to improve searchability for the remote file.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> Add(
        string externalId,
        string externalUrl,
        string title,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds a file from a remote service.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.add">Slack documentation</a> for more information.</remarks>
    /// <param name="externalId">Creator defined unique ID for the file.</param>
    /// <param name="externalUrl">URL of the remote file.</param>
    /// <param name="title">Title of the file being shared.</param>
    /// <param name="previewImage">Preview of the document.</param>
    /// <param name="filetype">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="indexableFileContents">Contents that can be used to improve searchability for the remote file.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> Add(
        string externalId,
        string externalUrl,
        string title,
        byte[] previewImage,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds a file from a remote service.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.add">Slack documentation</a> for more information.</remarks>
    /// <param name="externalId">Creator defined unique ID for the file.</param>
    /// <param name="externalUrl">URL of the remote file.</param>
    /// <param name="title">Title of the file being shared.</param>
    /// <param name="previewImage">Preview of the document.</param>
    /// <param name="filetype">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="indexableFileContents">Contents that can be used to improve searchability for the remote file.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> Add(
        string externalId,
        string externalUrl,
        string title,
        Stream previewImage,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieve information about a remote file added to Slack.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.info">Slack documentation</a> for more information.</remarks>
    /// <param name="externalId">Creator defined unique ID for the file.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> InfoByExternalId(string externalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve information about a remote file added to Slack.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.info">Slack documentation</a> for more information.</remarks>
    /// <param name="fileId">Specify a file by providing its ID.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> InfoByFileId(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists remote files visible to the token that calls it.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.list">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Filter files appearing in a specific channel, indicated by its ID.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="tsFrom">Filter files created after this timestamp (inclusive).</param>
    /// <param name="tsTo">Filter files created before this timestamp (inclusive).</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> attribute
    /// returned by a previous request's <see cref="FileListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<FileListResponse> List(
        string channelId = null,
        int limit = 100,
        string tsFrom = null,
        string tsTo = null,
        string cursor = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Removes a remote file from Slack. It does not delete the file from its external host.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.remove">Slack documentation</a> for more information.</remarks>
    /// <param name="externalId">Creator defined unique ID for the file.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveByExternalId(string externalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a remote file from Slack. It does not delete the file from its external host.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.remove">Slack documentation</a> for more information.</remarks>
    /// <param name="fileId">Specify a file by providing its ID.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveByFileId(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Share a remote file into a channel.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.share">Slack documentation</a> for more information.</remarks>
    /// <param name="externalId">Creator defined unique ID for the file.</param>
    /// <param name="channelIds">List of channel IDs where the file will be shared.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> ShareByExternalId(string externalId, IEnumerable<string> channelIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Share a remote file into a channel.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.share">Slack documentation</a> for more information.</remarks>
    /// <param name="fileId">Specify a file by providing its ID.</param>
    /// <param name="channelIds">List of channel IDs where the file will be shared.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> ShareByFileId(string fileId, IEnumerable<string> channelIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing remote file.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.update">Slack documentation</a> for more information.</remarks>
    /// <param name="externalId">Creator defined unique ID for the file.</param>
    /// <param name="externalUrl">URL of the remote file.</param>
    /// <param name="title">Title of the file being shared.</param>
    /// <param name="filetype">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="indexableFileContents">Contents that can be used to improve searchability for the remote file.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> UpdateByExternalId(
        string externalId,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing remote file.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.update">Slack documentation</a> for more information.</remarks>
    /// <param name="fileId">Specify a file by providing its ID.</param>
    /// <param name="externalUrl">URL of the remote file.</param>
    /// <param name="title">Title of the file being shared.</param>
    /// <param name="filetype">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="indexableFileContents">Contents that can be used to improve searchability for the remote file.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> UpdateByFileId(
        string fileId,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing remote file.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.update">Slack documentation</a> for more information.</remarks>
    /// <param name="externalId">Creator defined unique ID for the file.</param>
    /// <param name="externalUrl">URL of the remote file.</param>
    /// <param name="title">Title of the file being shared.</param>
    /// <param name="previewImage">Preview of the document.</param>
    /// <param name="filetype">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="indexableFileContents">Contents that can be used to improve searchability for the remote file.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> UpdateByExternalId(
        string externalId,
        byte[] previewImage,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing remote file.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.update">Slack documentation</a> for more information.</remarks>
    /// <param name="fileId">Specify a file by providing its ID.</param>
    /// <param name="externalUrl">URL of the remote file.</param>
    /// <param name="title">Title of the file being shared.</param>
    /// <param name="previewImage">Preview of the document.</param>
    /// <param name="filetype">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="indexableFileContents">Contents that can be used to improve searchability for the remote file.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> UpdateByFileId(
        string fileId,
        byte[] previewImage,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing remote file.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.update">Slack documentation</a> for more information.</remarks>
    /// <param name="externalId">Creator defined unique ID for the file.</param>
    /// <param name="externalUrl">URL of the remote file.</param>
    /// <param name="title">Title of the file being shared.</param>
    /// <param name="previewImage">Preview of the document.</param>
    /// <param name="filetype">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="indexableFileContents">Contents that can be used to improve searchability for the remote file.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> UpdateByExternalId(
        string externalId,
        Stream previewImage,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing remote file.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.remote.update">Slack documentation</a> for more information.</remarks>
    /// <param name="fileId">Specify a file by providing its ID.</param>
    /// <param name="externalUrl">URL of the remote file.</param>
    /// <param name="title">Title of the file being shared.</param>
    /// <param name="previewImage">Preview of the document.</param>
    /// <param name="filetype">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="indexableFileContents">Contents that can be used to improve searchability for the remote file.</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> UpdateByFileId(
        string fileId,
        Stream previewImage,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    );
}

public class RemoteFilesApi : IRemoteFilesApi
{
    private readonly ISlackApiClient _client;
    public RemoteFilesApi(ISlackApiClient client) => _client = client;

    public Task<FileResponse> Add(
        string externalId,
        string externalUrl,
        string title,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    ) =>
        Add(externalId, externalUrl, title, filetype, indexableFileContents, null, cancellationToken);

    public Task<FileResponse> Add(
        string externalId,
        string externalUrl,
        string title,
        byte[] previewImage,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    ) =>
        Add(externalId, externalUrl, title, filetype, indexableFileContents, new ByteArrayContent(previewImage), cancellationToken);

    public Task<FileResponse> Add(
        string externalId,
        string externalUrl,
        string title,
        Stream previewImage,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    ) =>
        Add(externalId, externalUrl, title, filetype, indexableFileContents, new StreamContent(previewImage), cancellationToken);

    private Task<FileResponse> Add(
        string externalId,
        string externalUrl,
        string title,
        string filetype,
        string indexableFileContents,
        HttpContent previewContent,
        CancellationToken cancellationToken
    ) =>
        AddOrUpdate("files.remote.add", new Args
            {
                { "external_id", externalId },
                { "external_url", externalUrl },
                { "title", title },
                { "filetype", filetype }
            }, indexableFileContents, previewContent, cancellationToken);

    public Task<FileResponse> InfoByExternalId(string externalId, CancellationToken cancellationToken = default) =>
        _client.Get<FileResponse>("files.remote.info", new Args { { "external_id", externalId } }, cancellationToken);

    public Task<FileResponse> InfoByFileId(string fileId, CancellationToken cancellationToken = default) =>
        _client.Get<FileResponse>("files.remote.info", new Args { { "file", fileId } }, cancellationToken);

    public Task<FileListResponse> List(
        string channelId = null,
        int limit = 100,
        string tsFrom = null,
        string tsTo = null,
        string cursor = null,
        CancellationToken cancellationToken = default
    ) =>
        _client.Get<FileListResponse>("files.remote.list", new Args
            {
                { "channel", channelId },
                { "limit", limit },
                { "ts_from", tsFrom },
                { "ts_to", tsTo },
                { "cursor", cursor }
            }, cancellationToken);

    public Task RemoveByExternalId(string externalId, CancellationToken cancellationToken = default) =>
        _client.Get("files.remote.remove", new Args { { "external_id", externalId } }, cancellationToken);

    public Task RemoveByFileId(string fileId, CancellationToken cancellationToken = default) =>
        _client.Get("files.remote.remove", new Args { { "file", fileId } }, cancellationToken);

    public Task<FileResponse> ShareByExternalId(string externalId, IEnumerable<string> channelIds, CancellationToken cancellationToken = default) =>
        _client.Get<FileResponse>("files.remote.share", new Args { { "external_id", externalId }, { "channels", channelIds } }, cancellationToken);

    public Task<FileResponse> ShareByFileId(string fileId, IEnumerable<string> channelIds, CancellationToken cancellationToken = default) =>
        _client.Get<FileResponse>("files.remote.share", new Args { { "file", fileId }, { "channels", channelIds } }, cancellationToken);

    public Task<FileResponse> UpdateByExternalId(
        string externalId,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    ) =>
        Update(externalId, null, externalUrl, title, filetype, indexableFileContents, null, cancellationToken);

    public Task<FileResponse> UpdateByFileId(
        string fileId,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    ) =>
        Update(null, fileId, externalUrl, title, filetype, indexableFileContents, null, cancellationToken);

    public Task<FileResponse> UpdateByExternalId(
        string externalId,
        byte[] previewImage,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    ) =>
        Update(externalId, null, externalUrl, title, filetype, indexableFileContents, new ByteArrayContent(previewImage), cancellationToken);

    public Task<FileResponse> UpdateByFileId(
        string fileId,
        byte[] previewImage,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    ) =>
        Update(null, fileId, externalUrl, title, filetype, indexableFileContents, new ByteArrayContent(previewImage), cancellationToken);

    public Task<FileResponse> UpdateByExternalId(
        string externalId,
        Stream previewImage,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    ) =>
        Update(externalId, null, externalUrl, title, filetype, indexableFileContents, new StreamContent(previewImage), cancellationToken);

    public Task<FileResponse> UpdateByFileId(
        string fileId,
        Stream previewImage,
        string externalUrl = null,
        string title = null,
        string filetype = null,
        string indexableFileContents = null,
        CancellationToken cancellationToken = default
    ) =>
        Update(null, fileId, externalUrl, title, filetype, indexableFileContents, new StreamContent(previewImage), cancellationToken);

    private Task<FileResponse> Update(
        string externalId,
        string fileId,
        string externalUrl,
        string title,
        string filetype,
        string indexableFileContents,
        HttpContent previewContent,
        CancellationToken cancellationToken
    ) =>
        AddOrUpdate("files.remote.update", new Args
            {
                { "external_id", externalId },
                { "file", fileId },
                { "external_url", externalUrl },
                { "title", title },
                { "filetype", filetype }
            }, indexableFileContents, previewContent, cancellationToken);

    private Task<FileResponse> AddOrUpdate(string slackMethod, Args args, string indexableFileContents, HttpContent previewContent, CancellationToken cancellationToken)
    {
        var content = new MultipartFormDataContent();

        if (indexableFileContents != null)
            content.Add(new StringContent(indexableFileContents), "indexable_file_contents", "index.txt");

        if (previewContent != null)
            content.Add(previewContent, "preview_image", "preview");

        return content.Any()
            ? _client.Post<FileResponse>(slackMethod, args, content, cancellationToken)
            : _client.Get<FileResponse>(slackMethod, args, cancellationToken);
    }
}