using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackNet.Events;
using SlackNet.Rtm;
using SlackNet.WebApi;
using WebSocket4Net;
using Reply = SlackNet.Rtm.Reply;

namespace SlackNet;

public interface ISlackRtmClient : IDisposable
{
    /// <summary>
    /// Messages coming from Slack.
    /// </summary>
    IObservable<MessageEvent> Messages { get; }

    /// <summary>
    /// All events (including messages) coming from Slack.
    /// </summary>
    IObservable<Event> Events { get; }

    /// <summary>
    /// Is the client connecting or has it connected.
    /// </summary>
    bool Connected { get; }

    /// <summary>
    /// Begins a Real Time Messaging API session and connects via a websocket.
    /// Will reconnect automatically.
    /// </summary>
    /// <param name="manualPresenceSubscription">Only deliver presence events when requested by subscription.</param>
    /// <param name="batchPresenceAware">Group presence change notices in <see cref="PresenceChange"/> events when possible.</param>
    /// <param name="cancellationToken"></param>
    Task<ConnectResponse> Connect(bool batchPresenceAware = false, bool manualPresenceSubscription = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send a simple message. For more complicated messages, use <see cref="ChatApi.PostMessage"/> instead.
    /// </summary>
    Task<MessageReply> SendMessage(string channelId, string text);

    /// <summary>
    /// Indicate that the user is currently writing a message to send to a channel.
    /// This can be sent on every key press in the chat input unless one has been sent in the last three seconds.
    /// </summary>
    void SendTyping(string channelId);

    /// <summary>
    /// Ask Slack's message server to subscribe you to presence events for the specified list of users.
    /// Calling again with fewer users will unsubscribe from the users no longer specified.
    /// </summary>
    void SetUserPresenceSubscription(IEnumerable<string> userIds);

    /// <summary>
    /// Ask the message server about the current presence status for the specified list of users.
    /// <see cref="PresenceChange"/> events will be sent back in response.
    /// </summary>
    void RequestUserPresence(IEnumerable<string> userIds);

    /// <summary>
    /// Low-level method for sending an arbitrary message over the websocket where a reply is expected.
    /// </summary>
    Task<Reply> SendWithReply(OutgoingRtmEvent slackEvent);

    /// <summary>
    /// Low-level method for sending an arbitrary message over the websocket where no reply is expected.
    /// </summary>
    void Send(OutgoingRtmEvent slackEvent);
}

public class SlackRtmClient : ISlackRtmClient
{
    private readonly SlackJsonSettings _jsonSettings;
    private readonly ISlackApiClient _client;
    private readonly ReconnectingWebSocket _webSocket;
    private readonly IObservable<Event> _rawEvents;
    private uint _nextEventId;

    public SlackRtmClient(string token)
    {
        _jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
        _client = new SlackApiClient(Default.Http(_jsonSettings), Default.UrlBuilder(_jsonSettings), _jsonSettings, token);
        _webSocket = new ReconnectingWebSocket(Default.WebSocketFactory, Scheduler.Default, Default.Logger, 0);
        _rawEvents = DeserializeEvents(_webSocket, _jsonSettings);
    }

    public SlackRtmClient(ISlackApiClient client, IWebSocketFactory webSocketFactory, SlackJsonSettings jsonSettings, IScheduler scheduler, ILogger logger)
    {
        _client = client;
        _jsonSettings = jsonSettings;
        _webSocket = new ReconnectingWebSocket(webSocketFactory, scheduler, logger, 0);
        _rawEvents = DeserializeEvents(_webSocket, _jsonSettings);
    }

    private static IObservable<Event> DeserializeEvents(ReconnectingWebSocket webSocket, SlackJsonSettings jsonSettings) =>
        webSocket.Messages
            .Select(m => JsonConvert.DeserializeObject<Event>(m.Message, jsonSettings.SerializerSettings))
            .Publish()
            .RefCount();

    /// <summary>
    /// Messages coming from Slack.
    /// </summary>
    public IObservable<MessageEvent> Messages => Events.OfType<MessageEvent>();

    /// <summary>
    /// All events (including messages) coming from Slack.
    /// </summary>
    public IObservable<Event> Events => _rawEvents.Where(e => !(e is Reply));

    /// <summary>
    /// Begins a Real Time Messaging API session and connects via a websocket.
    /// Will reconnect automatically.
    /// </summary>
    /// <param name="manualPresenceSubscription">Only deliver presence events when requested by subscription.</param>
    /// <param name="batchPresenceAware">Group presence change notices in <see cref="PresenceChange"/> events when possible.</param>
    /// <param name="cancellationToken"></param>
    public async Task<ConnectResponse> Connect(bool batchPresenceAware = false, bool manualPresenceSubscription = false, CancellationToken cancellationToken = default)
    {
        if (Connected)
            throw new InvalidOperationException("Already connecting or connected");

        ConnectResponse connectResponse = null;

        await _webSocket.Connect(
            async () =>
                {
                    connectResponse = await _client.Rtm.Connect(manualPresenceSubscription, batchPresenceAware, cancellationToken).ConfigureAwait(false);
                    return connectResponse.Url;
                },
            cancellationToken).ConfigureAwait(false);

        return connectResponse;
    }

    /// <summary>
    /// Is the client connecting or has it connected.
    /// </summary>
    public bool Connected => _webSocket?.State
        is WebSocketState.Connecting
        or WebSocketState.Open;

    /// <summary>
    /// Send a simple message. For more complicated messages, use <see cref="ChatApi.PostMessage"/> instead.
    /// </summary>
    public async Task<MessageReply> SendMessage(string channelId, string text)
    {
        var reply = await SendWithReply(new RtmMessage { Channel = channelId, Text = text }).ConfigureAwait(false);
        return new MessageReply
            {
                Text = reply.Text,
                Ts = reply.Ts
            };
    }

    /// <summary>
    /// Indicate that the user is currently writing a message to send to a channel.
    /// This can be sent on every key press in the chat input unless one has been sent in the last three seconds.
    /// </summary>
    public void SendTyping(string channelId) => Send(new Typing { Channel = channelId });

    /// <summary>
    /// Ask Slack's message server to subscribe you to presence events for the specified list of users.
    /// Calling again with fewer users will unsubscribe from the users no longer specified.
    /// </summary>
    public void SetUserPresenceSubscription(IEnumerable<string> userIds) =>
        Send(new PresenceSub { Ids = userIds.ToList() });

    /// <summary>
    /// Ask the message server about the current presence status for the specified list of users.
    /// <see cref="PresenceChange"/> events will be sent back in response.
    /// </summary>
    public void RequestUserPresence(IEnumerable<string> userIds) =>
        Send(new PresenceQuery { Ids = userIds.ToList() });

    /// <summary>
    /// Low-level method for sending an arbitrary message over the websocket where a reply is expected.
    /// </summary>
    public async Task<Reply> SendWithReply(OutgoingRtmEvent slackEvent)
    {
        var json = Serialize(slackEvent, out var id);
        var reply = _rawEvents.OfType<Reply>()
            .Where(r => r.ReplyTo == id)
            .FirstOrDefaultAsync()
            .ToTask();
        Send(json);
        return await reply.ConfigureAwait(false);
    }

    /// <summary>
    /// Low-level method for sending an arbitrary message over the websocket where no reply is expected.
    /// </summary>
    public void Send(OutgoingRtmEvent slackEvent) => Send(Serialize(slackEvent, out _));

    private JObject Serialize(OutgoingRtmEvent slackEvent, out uint id)
    {
        var json = JObject.FromObject(slackEvent, JsonSerializer.Create(_jsonSettings.SerializerSettings));
        id = _nextEventId++;
        json["id"] = id;
        return json;
    }

    private void Send(JObject json) => _webSocket.Send(json.ToString(Formatting.None, _jsonSettings.SerializerSettings.Converters.ToArray()));

    public void Dispose()
    {
        _webSocket?.Dispose();
    }
}