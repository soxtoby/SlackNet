#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Retrieves analytics data for an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminAnalyticsApi
{
    /// <summary>
    /// Retrieves member analytics for a given date. Each record provides information about a different user.
    /// </summary>
    /// <remarks>
    /// Slack provides the analytics as a compressed file, which is read as the records are enumerated.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.getFile/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="date">Date to retrieve the analytics data for, in UTC.</param>
    /// <param name="cancellationToken"></param>
    IAsyncEnumerable<MemberAnalytics> GetMemberAnalytics(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves public channel analytics for a given date. Each record provides information about activity in a single public channel.
    /// Archived and deleted channels are not included.
    /// </summary>
    /// <remarks>
    /// Slack provides the analytics as a compressed file, which is read as the records are enumerated.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.getFile/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="date">Date to retrieve the analytics data for, in UTC.</param>
    /// <param name="cancellationToken"></param>
    IAsyncEnumerable<PublicChannelAnalytics> GetPublicChannelAnalytics(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves metadata about the public channels appearing in public channel analytics, such as their names.
    /// Metadata cannot be filtered by date.
    /// </summary>
    /// <remarks>
    /// Slack provides the metadata as a compressed file, which is read as the records are enumerated.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.getFile/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="cancellationToken"></param>
    IAsyncEnumerable<PublicChannelMetadata> GetPublicChannelMetadata(CancellationToken cancellationToken = default);
}

public class AdminAnalyticsApi(ISlackApiClient client, SlackJsonSettings jsonSettings) : IAdminAnalyticsApi
{
    public IAsyncEnumerable<MemberAnalytics> GetMemberAnalytics(DateTime date, CancellationToken cancellationToken = default) =>
        GetFile<MemberAnalytics>(new Args
            {
                { "type", "member" },
                { "date", date }
            }, cancellationToken);

    public IAsyncEnumerable<PublicChannelAnalytics> GetPublicChannelAnalytics(DateTime date, CancellationToken cancellationToken = default) =>
        GetFile<PublicChannelAnalytics>(new Args
            {
                { "type", "public_channel" },
                { "date", date }
            }, cancellationToken);

    public IAsyncEnumerable<PublicChannelMetadata> GetPublicChannelMetadata(CancellationToken cancellationToken = default) =>
        GetFile<PublicChannelMetadata>(new Args
            {
                { "type", "public_channel" },
                { "metadata_only", true }
            }, cancellationToken);

    /// <summary>
    /// Reads a gzipped file of newline-delimited JSON records, one line at a time, so that large files don't need to fit in memory.
    /// </summary>
    private async IAsyncEnumerable<T> GetFile<T>(Args args, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var file = await client.GetStream("admin.analytics.getFile", args, cancellationToken).ConfigureAwait(false);
        using var decompressed = new GZipStream(file, CompressionMode.Decompress);
        using var reader = new StreamReader(decompressed);

        while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
        {
            if (!string.IsNullOrWhiteSpace(line))
                yield return JsonConvert.DeserializeObject<T>(line, jsonSettings.SerializerSettings)!;
        }
    }
}
