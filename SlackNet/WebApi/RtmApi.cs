using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IRtmApi
{
    /// <summary>
    /// Begins a Real Time Messaging API session and reserves your application a specific URL with which to connect via websocket.
    /// Unlike <see cref="RtmApi.Start"/>, this method is focused only on connecting to the RTM API.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/rtm.connect/">Slack documentation</a> for more information.</remarks>
    /// <param name="manualPresenceSubscription">Only deliver presence events when requested by subscription.</param>
    /// <param name="batchPresenceAware">Group presence change notices in <see cref="PresenceChange"/> events when possible.</param>
    /// <param name="cancellationToken"></param>
    Task<ConnectResponse> Connect(bool manualPresenceSubscription = false, bool batchPresenceAware = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a Real Time Messaging API session and reserves your application a specific URL with which to connect via websocket.
    /// This method also returns a smorgasbord of data about the team, its channels, and members. Some times more information than can be provided in a timely or helpful manner.
    /// Please use <see cref="RtmApi.Connect"/> instead, especially when connecting on behalf of an Enterprise Grid customer.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/rtm.start/">Slack documentation</a> for more information.</remarks>
    /// <param name="simpleLatest">Return timestamp only for latest message object of each channel (improves performance).</param>
    /// <param name="noUnreads">Skip unread counts for each channel (improves performance).</param>
    /// <param name="mpimAware">Returns MPIMs to the client in the API response.</param>
    /// <param name="manualPresenceSubscription">Only deliver presence events when requested by subscription.</param>
    /// <param name="batchPresenceAware">Group presence change notices in <see cref="PresenceChange"/> events when possible.</param>
    /// <param name="includeLocale">Set this to true to receive the locale for users and channels.</param>
    /// <param name="noLatest">Exclude latest timestamps for channels, groups, mpims, and ims. Automatically sets <paramref name="noUnreads"/> to False.</param>
    /// <param name="cancellationToken"></param>
    Task<StartResponse> Start(
        bool simpleLatest = false,
        bool noUnreads = false,
        bool mpimAware = false,
        bool manualPresenceSubscription = false,
        bool batchPresenceAware = false,
        bool includeLocale = false,
        bool noLatest = false,
        CancellationToken cancellationToken = default
    );
}

public class RtmApi(ISlackApiClient client) : IRtmApi
{
    public Task<ConnectResponse> Connect(bool manualPresenceSubscription = false, bool batchPresenceAware = false, CancellationToken cancellationToken = default) =>
        client.Get<ConnectResponse>("rtm.connect", new Args { { "presence_sub", manualPresenceSubscription }, { "batch_presence_aware", batchPresenceAware } }, cancellationToken);

    public Task<StartResponse> Start(
        bool simpleLatest = false,
        bool noUnreads = false,
        bool mpimAware = false,
        bool manualPresenceSubscription = false,
        bool batchPresenceAware = false,
        bool includeLocale = false,
        bool noLatest = false,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<StartResponse>("rtm.start", new Args
                {
                    { "simple_latest", simpleLatest },
                    { "no_unreads", noUnreads },
                    { "mpim_aware", mpimAware },
                    { "presence_sub", manualPresenceSubscription },
                    { "batch_presence_aware", batchPresenceAware },
                    { "include_locale", includeLocale },
                    { "no_latest", noLatest }
                },
            cancellationToken);
}