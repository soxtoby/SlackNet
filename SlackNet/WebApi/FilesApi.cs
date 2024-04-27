using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IFilesApi
{
    /// <summary>
    /// Deletes a file from your team.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.delete">Slack documentation</a> for more information.</remarks>
    /// <param name="fileId">ID of file to delete.</param>
    /// <param name="cancellationToken"></param>
    Task Delete(string fileId, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Returns information about a file in your team.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.info">Slack documentation</a> for more information.</remarks>
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
    /// <remarks>See the <a href="https://api.slack.com/methods/files.list">Slack documentation</a> for more information.</remarks>
    /// <param name="userId">Filter files created by a single user.</param>
    /// <param name="channelId">Filter files appearing in a specific channel, indicated by its ID.</param>
    /// <param name="tsFrom">Filter files created after this timestamp (inclusive).</param>
    /// <param name="tsTo">Filter files created before this timestamp (inclusive).</param>
    /// <param name="types">Filter files by type.</param>
    /// <param name="count">Number of items to return per page.</param>
    /// <param name="page">Page number of results to return.</param>
    /// <param name="cursor">
    /// Parameter for pagination.
    /// Set cursor equal to the <see cref="ResponseMetadata.NextCursor"/> returned by the previous request's <see cref="ReactionItemListResponse.ResponseMetadata"/>.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<FileListResponse> List(
        string userId = null,
        string channelId = null,
        string tsFrom = null,
        string tsTo = null,
        IEnumerable<FileType> types = null,
        int count = 100,
        int page = 1,
        string cursor = null,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Disables public/external sharing for a file.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.revokePublicURL">Slack documentation</a> for more information.</remarks>
    /// <param name="fileId">File to revoke</param>
    /// <param name="cancellationToken"></param>
    Task<FileResponse> RevokePublicUrl(string fileId, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Enables public/external sharing for a file.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.sharedPublicURL">Slack documentation</a> for more information.</remarks>
    /// <param name="fileId">File to share.</param>
    /// <param name="cancellationToken"></param>
    Task<FileAndCommentsResponse> SharedPublicUrl(string fileId, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Allows you to create or upload an existing file.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.upload">Slack documentation</a> for more information.</remarks>
    /// <param name="fileContents">Contents of text file.</param>
    /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="fileName">Filename of file.</param>
    /// <param name="title">Title of file</param>
    /// <param name="initialComment">Initial comment to add to file.</param>
    /// <param name="threadTs">Provide another message's <see cref="MessageEventBase.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
    /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
    /// <param name="cancellationToken"></param>
    [Obsolete("Use the overload that takes an ExternalFile instead.")]
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
    /// <remarks>See the <a href="https://api.slack.com/methods/files.upload">Slack documentation</a> for more information.</remarks>
    /// <param name="fileContents">Contents of file.</param>
    /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="fileName">Filename of file.</param>
    /// <param name="title">Title of file</param>
    /// <param name="initialComment">Initial comment to add to file.</param>
    /// <param name="threadTs">Provide another message's <see cref="MessageEventBase.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
    /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
    /// <param name="cancellationToken"></param>
    [Obsolete("Use the overload that takes an ExternalFile instead.")]
    Task<FileResponse> Upload(
        byte[] fileContents,
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
    /// <remarks>See the <a href="https://api.slack.com/methods/files.upload">Slack documentation</a> for more information.</remarks>
    /// <param name="fileContents">Contents of file.</param>
    /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="fileName">Filename of file.</param>
    /// <param name="title">Title of file</param>
    /// <param name="initialComment">Initial comment to add to file.</param>
    /// <param name="threadTs">Provide another message's <see cref="MessageEventBase.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
    /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
    /// <param name="cancellationToken"></param>
    [Obsolete("Use the overload that takes an ExternalFile instead.")]
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
    /// <remarks>See the <a href="https://api.slack.com/methods/files.upload">Slack documentation</a> for more information.</remarks>
    /// <param name="snippet">Contents file text file.</param>
    /// <param name="fileType">A file type identifier (see https://api.slack.com/types/file#file_types for more information).</param>
    /// <param name="fileName">Filename of file.</param>
    /// <param name="title">Title of file</param>
    /// <param name="initialComment">Initial comment to add to file.</param>
    /// <param name="threadTs">Provide another message's <see cref="MessageEventBase.Ts"/> value to upload this file as a reply. Never use a reply's <c>Ts</c> value; use its parent instead.</param>
    /// <param name="channels">List of channel names or IDs where the file will be shared.</param>
    /// <param name="cancellationToken"></param>
    [Obsolete("Use the overload of Upload that takes an ExternalFile instead.")]
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

    /// <summary>
    /// Upload a single external file.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/messaging/files#uploading_files">Slack documentation</a> for more information.</remarks>
    /// <param name="file">An external file to upload.</param>
    /// <param name="channelId">Channel ID where the file will be shared. If not specified the file will be private.</param>
    /// <param name="threadTs">Provide another message's <see cref="MessageEventBase.Ts"/> value to upload this file as a reply. Never use a reply's ts value; use its parent instead.</param>
    /// <param name="initialComment">The message text introducing the file in specified channels.</param>
    /// <param name="cancellationToken"></param>
    Task<ExternalFileReference> Upload(
        FileUpload file,
        string channelId = null,
        string threadTs = null,
        string initialComment = null,
        CancellationToken? cancellationToken = null);

    /// <summary>
    /// Uploads external files.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/messaging/files#uploading_files">Slack documentation</a> for more information.</remarks>
    /// <param name="files">List of external files to upload.</param>
    /// <param name="channelId">Channel ID where the files will be shared. If not specified the files will be private.</param>
    /// <param name="threadTs">Provide another message's <see cref="MessageEventBase.Ts"/> value to upload these files as a reply. Never use a reply's <see cref="MessageEventBase.Ts"/> value; use its parent instead.</param>
    /// <param name="initialComment">The message text introducing the files in the specified channel.</param>
    /// <param name="cancellationToken"></param>
    Task<IList<ExternalFileReference>> Upload(
        IEnumerable<FileUpload> files,
        string channelId = null,
        string threadTs = null,
        string initialComment = null,
        CancellationToken? cancellationToken = null);

    /// <summary>
    /// Gets a URL for an edge external file upload.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.getUploadURLExternal">Slack documentation</a> for more information.</remarks>
    /// <param name="fileName">Name of the file being uploaded.</param>
    /// <param name="length">Size in bytes of the file being uploaded.</param>
    /// <param name="altText">Description of image for screen-reader.</param>
    /// <param name="snippetType">Syntax type of the snippet being uploaded.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UploadUrlExternalResponse> GetUploadUrlExternal(
        string fileName,
        int length,
        string altText = null,
        string snippetType = null,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Finishes an upload started with <see cref="GetUploadUrlExternal"/>.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/files.completeUploadExternal">Slack documentation</a> for more information.</remarks>
    /// <param name="files">List of file ids and their corresponding (optional) titles.</param>
    /// <param name="channelId">Channel ID where the files will be shared. If not specified the files will be private.</param>
    /// <param name="initialComment">The message text introducing the files in specified channels.</param>
    /// <param name="threadTs">Provide another message's <see cref="MessageEventBase.Ts"/> value to upload these files as a reply. Never use a reply's <see cref="MessageEventBase.Ts"/> value; use its parent instead.</param>
    /// <param name="cancellationToken"></param>
    Task<IList<ExternalFileReference>> CompleteUploadExternal(
        IEnumerable<ExternalFileReference> files,
        string channelId = null,
        string initialComment = null,
        string threadTs = null,
        CancellationToken? cancellationToken = null);
}

public class FilesApi(ISlackApiClient client, IHttp http) : IFilesApi
{
    [Obsolete("Include IHttp parameter.")]
    public FilesApi(ISlackApiClient client) : this(client, Default.Http()) { }
    
    public Task Delete(string fileId, CancellationToken? cancellationToken = null) =>
        client.Post("files.delete", new Args { { "file", fileId } }, cancellationToken);

    public Task<FileAndCommentsResponse> Info(string fileId, int count = 100, int page = 1, string cursor = null, CancellationToken? cancellationToken = null) =>
        client.Get<FileAndCommentsResponse>("files.info", new Args
            {
                { "file", fileId },
                { "count", count },
                { "page", page },
                { "cursor", cursor }
            }, cancellationToken);

    public Task<FileListResponse> List(
        string userId = null,
        string channelId = null,
        string tsFrom = null,
        string tsTo = null,
        IEnumerable<FileType> types = null,
        int count = 100,
        int page = 1,
        string cursor = null,
        CancellationToken? cancellationToken = null
    ) =>
        client.Get<FileListResponse>("files.list", new Args
            {
                { "user", userId },
                { "channel", channelId },
                { "ts_from", tsFrom },
                { "ts_to", tsTo },
                { "types", types },
                { "count", count },
                { "page", page },
                { "cursor", cursor }
            }, cancellationToken);

    public Task<FileResponse> RevokePublicUrl(string fileId, CancellationToken? cancellationToken = null) =>
        client.Post<FileResponse>("files.revokePublicURL", new Args { { "file", fileId } }, cancellationToken);

    public Task<FileAndCommentsResponse> SharedPublicUrl(string fileId, CancellationToken? cancellationToken = null) =>
        client.Post<FileAndCommentsResponse>("files.sharedPublicURL", new Args { { "file", fileId } }, cancellationToken);

    public async Task<FileResponse> Upload(
        string fileContents,
        string fileType = null,
        string fileName = null,
        string title = null,
        string initialComment = null,
        string threadTs = null,
        IEnumerable<string> channels = null,
        CancellationToken? cancellationToken = null)
    {
        using var content = new StringContent(fileContents);
        return await Upload(content, fileType, fileName, title, initialComment, threadTs, channels, cancellationToken).ConfigureAwait(false);
    }

    public async Task<FileResponse> Upload(
        byte[] fileContents,
        string fileType = null,
        string fileName = null,
        string title = null,
        string initialComment = null,
        string threadTs = null,
        IEnumerable<string> channels = null,
        CancellationToken? cancellationToken = null)
    {
        using var content = new ByteArrayContent(fileContents);
        return await Upload(content, fileType, fileName, title, initialComment, threadTs, channels, cancellationToken).ConfigureAwait(false);
    }

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

    private async Task<FileResponse> Upload(
        HttpContent fileContent,
        string fileType,
        string fileName,
        string title,
        string initialComment,
        string threadTs,
        IEnumerable<string> channels,
        CancellationToken? cancellationToken
    )
    {
        using var content = new MultipartFormDataContent();
        content.Add(fileContent, "file", fileName ?? "file");

        return await client.Post<FileResponse>("files.upload", new Args
                {
                    { "filetype", fileType },
                    { "filename", fileName },
                    { "title", title },
                    { "initial_comment", initialComment },
                    { "channels", channels },
                    { "thread_ts", threadTs }
                },
            content,
            cancellationToken).ConfigureAwait(false);
    }

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
        client.Post<FileResponse>("files.upload", new Args(), new SlackFormContent
                {
                    { "filetype", fileType },
                    { "filename", fileName },
                    { "title", title },
                    { "initial_comment", initialComment },
                    { "channels", channels is null ? null : string.Join(",", channels) },
                    { "thread_ts", threadTs },
                    { "content", snippet }
                },
            cancellationToken);
    
    public async Task<ExternalFileReference> Upload(
        FileUpload fileUpload,
        string channelId = null,
        string threadTs = null,
        string initialComment = null,
        CancellationToken? cancellationToken = null
    ) => (await Upload([fileUpload], channelId, threadTs, initialComment, cancellationToken).ConfigureAwait(false)).Single();

    public async Task<IList<ExternalFileReference>> Upload(IEnumerable<FileUpload> files, string channelId = null, string threadTs = null, string initialComment = null, CancellationToken? cancellationToken = null)
    {
        var fileReferences = await Task.WhenAll(files
            .Select(async file =>
                {
                    using var content = file.GetContent();
                    var length = content.HttpContent.Headers.ContentLength ?? throw new InvalidOperationException($"Couldn't get length of file {file.FileName}");
                    var uploadUrlResponse = await GetUploadUrlExternal(file.FileName, Convert.ToInt32(length), file.AltText, file.SnippetType, cancellationToken).ConfigureAwait(false);
                    await http.Execute<WebApiResponse>(new HttpRequestMessage(HttpMethod.Post, uploadUrlResponse.UploadUrl) { Content = content.HttpContent }, cancellationToken).ConfigureAwait(false);
                    return new ExternalFileReference { Id = uploadUrlResponse.FileId, Title = file.Title };
                })).ConfigureAwait(false);

        return await CompleteUploadExternal(fileReferences, channelId, threadTs, initialComment, cancellationToken).ConfigureAwait(false);
    }

    public Task<UploadUrlExternalResponse> GetUploadUrlExternal(
        string fileName,
        int length,
        string altText = null,
        string snippetType = null,
        CancellationToken? cancellationToken = null
    ) =>
        client.Get<UploadUrlExternalResponse>("files.getUploadURLExternal", new Args
            {
                { "filename", fileName },
                { "length", length },
                { "alt_text", altText },
                { "snippet_type", snippetType }
            }, cancellationToken);

    public async Task<IList<ExternalFileReference>> CompleteUploadExternal(
        IEnumerable<ExternalFileReference> files,
        string channelId = null,
        string threadTs = null,
        string initialComment = null,
        CancellationToken? cancellationToken = null
    ) =>
        (await client.Post<CompleteUploadExternalResponse>("files.completeUploadExternal", new Args
            {
                { "files", files },
                { "channel_id", channelId },
                { "initial_comment", initialComment },
                { "thread_ts", threadTs }
            }, cancellationToken).ConfigureAwait(false))
        .Files;
}