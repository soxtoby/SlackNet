using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IChatApi
{
    /// <summary>
    /// Deletes a message from a channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.delete/">Slack documentation</a> for more information.</remarks>
    /// <param name="ts">Timestamp of the message to be deleted.</param>
    /// <param name="channelId">Channel containing the message to be deleted.</param>
    /// <param name="cancellationToken"></param>
    Task<MessageTsResponse> Delete(string ts, string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a /me message to a channel from the calling user.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.meMessage/">Slack documentation</a> for more information.</remarks>
    /// <param name="channel">Channel to send message to. Can be a public channel, private group or IM channel. Can be an encoded ID, or a name.</param>
    /// <param name="text">Text of the message to send.</param>
    /// <param name="cancellationToken"></param>
    Task<MessageTsResponse> MeMessage(string channel, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Posts a message to a public channel, private channel, or direct message/IM channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.postMessage/">Slack documentation</a> for more information.</remarks>
    /// <param name="message">The message to post</param>
    /// <param name="cancellationToken"></param>
    Task<PostMessageResponse> PostMessage(Message message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules a message for delivery to a public channel, private channel, or direct message/IM channel at a specified time in the future.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.scheduleMessage/">Slack documentation</a> for more information.</remarks>
    /// <param name="message">The message to post.</param>
    /// <param name="postAt">Time in the future to send the message.</param>
    /// <param name="cancellationToken"></param>
    Task<ScheduleMessageResponse> ScheduleMessage(Message message, DateTime postAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a scheduled message.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.deleteScheduledMessage/">Slack documentation</a> for more information.</remarks>
    /// <param name="messageId">The ID of the scheduled message.</param>
    /// <param name="channelId">The channel ID of the scheduled message.</param>
    /// <param name="asUser">Pass True to delete the message as the authed user. Bot users in this context are considered authed users.</param>
    /// <param name="cancellationToken"></param>
    Task DeleteScheduledMessage(string messageId, string channelId, bool? asUser = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an ephemeral message to a user in a channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.postEphemeral/">Slack documentation</a> for more information.</remarks>
    /// <param name="userId">ID of the user who will receive the ephemeral message. The user should be in the channel specified by the channel argument.</param>
    /// <param name="message">The message to post. Not all message properties are supported by <c>PostEphemeral</c>.</param>
    /// <param name="cancellationToken"></param>
    Task<PostEphemeralResponse> PostEphemeral(string userId, Message message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attaches Slack app unfurl behavior to a specified and relevant message.
    /// </summary>
    /// <param name="channelId">Channel ID of the message.</param>
    /// <param name="ts">Timestamp of the message to add unfurl behavior to.</param>
    /// <param name="unfurls">Dictionary mapping a set of URLs from the message to their unfurl attachment.</param>
    /// <param name="userAuthRequired">Set to True to indicate the user must install your Slack app to trigger unfurls for this domain.</param>
    /// <param name="userAuthBlocks">Structured blocks to send as an ephemeral message to the user as invitation to authenticate further and enable full unfurling behavior.</param>
    /// <param name="userAuthMessage">A simply-formatted string to send as an ephemeral message to the user as invitation to authenticate further and enable full unfurling behavior.</param>
    /// <param name="userAuthUrl">Send users to this custom URL where they will complete authentication in your app to fully trigger unfurling. Value should be properly URL-encoded.</param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// <para>
    /// The first time this method is executed with a particular <paramref name="ts"/> and <paramref name="channelId"/> combination,
    /// the valid <paramref name="unfurls"/> attachments you provide will be attached to the message.
    /// Subsequent attempts with the same <paramref name="ts"/> and <paramref name="channelId"/> values will modify the same attachments, rather than adding more.
    /// </para>
    /// <para>
    /// See the <a href="https://docs.slack.dev/reference/methods/chat.unfurl/">Slack documentation</a> for more information.
    /// </para>
    /// </remarks>
    Task Unfurl(
        string channelId,
        string ts,
        IDictionary<string, Attachment> unfurls,
        bool userAuthRequired = false,
        IEnumerable<Block> userAuthBlocks = null,
        string userAuthMessage = null,
        string userAuthUrl = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Attaches Slack app unfurl behavior to a specified and relevant message.
    /// </summary>
    /// <param name="source">The source of the link to unfurl.</param>
    /// <param name="unfurlId">The ID of the link to unfurl.</param>
    /// <param name="unfurls">Dictionary mapping a set of URLs from the message to their unfurl attachment.</param>
    /// <param name="userAuthRequired">Set to True to indicate the user must install your Slack app to trigger unfurls for this domain.</param>
    /// <param name="userAuthBlocks">Structured blocks to send as an ephemeral message to the user as invitation to authenticate further and enable full unfurling behavior.</param>
    /// <param name="userAuthMessage">A simply-formatted string to send as an ephemeral message to the user as invitation to authenticate further and enable full unfurling behavior.</param>
    /// <param name="userAuthUrl">Send users to this custom URL where they will complete authentication in your app to fully trigger unfurling. Value should be properly URL-encoded.</param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// <para>
    /// The first time this method is executed with a particular <paramref name="source"/> and <paramref name="unfurlId"/> combination,
    /// the valid <paramref name="unfurls"/> attachments you provide will be attached to the message.
    /// Subsequent attempts with the same <paramref name="source"/> and <paramref name="unfurlId"/> values will modify the same attachments, rather than adding more.
    /// </para>
    /// <para>
    /// See the <a href="https://docs.slack.dev/reference/methods/chat.unfurl/">Slack documentation</a> for more information.
    /// </para>
    /// </remarks>
    Task Unfurl(
        LinkSource source,
        string unfurlId,
        IDictionary<string, Attachment> unfurls,
        bool userAuthRequired = false,
        IEnumerable<Block> userAuthBlocks = null,
        string userAuthMessage = null,
        string userAuthUrl = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a message in a channel.
    /// </summary>
    /// <param name="messageUpdate">Message to update.</param>
    /// <param name="cancellationToken"></param>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.update/">Slack documentation</a> for more information.</remarks>
    Task<MessageUpdateResponse> Update(MessageUpdate messageUpdate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve a permalink URL for a specific extant message.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.getPermalink/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">The ID of the conversation or channel containing the message.</param>
    /// <param name="messageTs">A message's timestamp, uniquely identifying it within a channel.</param>
    /// <param name="cancellationToken"></param>
    Task<PermalinkResponse> GetPermalink(string channelId, string messageTs, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a new streaming conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.startStream/">Slack documentation</a> for more information.</remarks>
    /// <param name="channel">An encoded ID that represents a channel, private group, or DM.</param>
    /// <param name="threadTs">Provide another message's timestamp value to reply to. Streamed messages should always be replies to a user request.</param>
    /// <param name="markdownText">Accepts message text formatted in markdown. This text is what will be appended to the message received so far.</param>
    /// <param name="recipientUserId">The encoded ID of the user to receive the streaming text. Required when streaming to channels.</param>
    /// <param name="recipientTeamId">The encoded ID of the team the user receiving the streaming text belongs to. Required when streaming to channels.</param>
    /// <param name="cancellationToken"></param>
    Task<MessageTsResponse> StartStream(string channel, string threadTs, string markdownText = null, string recipientUserId = null, string recipientTeamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends text to an existing streaming conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.appendStream/">Slack documentation</a> for more information.</remarks>
    /// <param name="channel">An encoded ID that represents a channel, private group, or DM.</param>
    /// <param name="ts">The timestamp of the streaming message.</param>
    /// <param name="markdownText">Accepts message text formatted in markdown. This text is what will be appended to the message received so far.</param>
    /// <param name="cancellationToken"></param>
    Task<MessageTsResponse> AppendStream(string channel, string ts, string markdownText, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops a streaming conversation.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.stopStream/">Slack documentation</a> for more information.</remarks>
    /// <param name="channel">An encoded ID that represents a channel, private group, or DM.</param>
    /// <param name="ts">A message's timestamp, uniquely identifying it within a channel.</param>
    /// <param name="markdownText">Accepts message text formatted in markdown. This text is what will be appended to the message received so far.</param>
    /// <param name="blocks">A list of blocks that will be rendered at the bottom of the finalized message.</param>
    /// <param name="metadataObject">
    /// A custom event attached to the message.
    /// Metadata you post to Slack is accessible to any app or user who is a member of that workspace.
    /// The specified object will be automatically converted to a <see cref="MessageMetadata"/> using the standard Slack JSON conventions.
    /// </param>
    /// <param name="metadataJson">
    /// A custom event attached to the message.
    /// Metadata you post to Slack is accessible to any app or user who is a member of that workspace.
    /// Will take precedence over <see cref="metadataObject"/>.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<PostMessageResponse> StopStream(string channel, string ts, string markdownText = null, IEnumerable<Block> blocks = null, object metadataObject = null, MessageMetadata metadataJson = null, CancellationToken cancellationToken = default);
}

public class ChatApi(ISlackApiClient client, SlackJsonSettings jsonSettings) : IChatApi
{
    public Task<MessageTsResponse> Delete(string ts, string channelId, CancellationToken cancellationToken = default) =>
        client.Post<MessageTsResponse>("chat.delete", new Args
            {
                { "ts", ts },
                { "channel", channelId }
            }, cancellationToken);

    public Task<PermalinkResponse> GetPermalink(string channelId, string messageTs, CancellationToken cancellationToken = default) =>
        client.Get<PermalinkResponse>("chat.getPermalink", new Args
            {
                { "channel", channelId },
                { "message_ts", messageTs }
            }, cancellationToken);

    public Task<MessageTsResponse> MeMessage(string channel, string text, CancellationToken cancellationToken = default) =>
        client.Post<MessageTsResponse>("chat.meMessage", new Args
            {
                { "channel", channel },
                { "text", text }
            }, cancellationToken);

    public Task<PostMessageResponse> PostMessage(Message message, CancellationToken cancellationToken = default) =>
        client.Post<PostMessageResponse>("chat.postMessage", PopulateMessageArgs(message, new Args()), cancellationToken);

    public Task<ScheduleMessageResponse> ScheduleMessage(Message message, DateTime postAt, CancellationToken cancellationToken = default) =>
        client.Post<ScheduleMessageResponse>("chat.scheduleMessage", PopulateMessageArgs(message, new Args
                {
                    { "post_at", postAt.ToTimestamp() }
                }),
            cancellationToken);

    private Args PopulateMessageArgs(Message message, Args args)
    {
        args["channel"] = message.Channel;
        args["text"] = message.Text;
        args["parse"] = message.Parse;
        args["link_names"] = message.LinkNames;
        args["attachments"] = message.Attachments;
        args["blocks"] = message.Blocks;
        args["unfurl_links"] = message.UnfurlLinks;
        args["unfurl_media"] = message.UnfurlMedia;
        args["username"] = message.Username;
        args["as_user"] = message.AsUser;
        args["icon_url"] = message.IconUrl;
        args["icon_emoji"] = message.IconEmoji;
        args["thread_ts"] = message.ThreadTs;
        args["reply_broadcast"] = message.ReplyBroadcast;
        args["metadata"] = message.MetadataJson ?? MessageMetadata.FromObject(message.MetadataObject, jsonSettings);
        return args;
    }

    public Task DeleteScheduledMessage(string messageId, string channelId, bool? asUser = null, CancellationToken cancellationToken = default) =>
        client.Post("chat.deleteScheduledMessage", new Args
                {
                    { "scheduled_message_id", messageId },
                    { "channel", channelId },
                    { "as_user", asUser }
                },
            cancellationToken);

    public Task<PostEphemeralResponse> PostEphemeral(string userId, Message message, CancellationToken cancellationToken = default) =>
        client.Post<PostEphemeralResponse>("chat.postEphemeral", new Args
                {
                    { "channel", message.Channel },
                    { "text", message.Text },
                    { "user", userId },
                    { "as_user", message.AsUser },
                    { "attachments", message.Attachments },
                    { "blocks", message.Blocks },
                    { "link_names", message.LinkNames },
                    { "thread_ts", message.ThreadTs },
                    { "parse", message.Parse }
                },
            cancellationToken);

    public Task Unfurl(
        string channelId,
        string ts,
        IDictionary<string, Attachment> unfurls,
        bool userAuthRequired = false,
        IEnumerable<Block> userAuthBlocks = null,
        string userAuthMessage = null,
        string userAuthUrl = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("chat.unfurl", new Args
            {
                { "channel", channelId },
                { "ts", ts },
                { "unfurls", unfurls },
                { "user_auth_required", userAuthRequired },
                { "user_auth_blocks", userAuthBlocks },
                { "user_auth_message", userAuthMessage },
                { "user_auth_url", userAuthUrl }
            }, cancellationToken);

    public Task Unfurl(
        LinkSource source,
        string unfurlId,
        IDictionary<string, Attachment> unfurls,
        bool userAuthRequired = false,
        IEnumerable<Block> userAuthBlocks = null,
        string userAuthMessage = null,
        string userAuthUrl = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("chat.unfurl", new Args
            {
                { "source", source },
                { "unfurl_id", unfurlId },
                { "unfurls", unfurls },
                { "user_auth_required", userAuthRequired },
                { "user_auth_blocks", userAuthBlocks },
                { "user_auth_message", userAuthMessage },
                { "user_auth_url", userAuthUrl }
            }, cancellationToken);

    public Task<MessageUpdateResponse> Update(MessageUpdate messageUpdate, CancellationToken cancellationToken = default) =>
        client.Post<MessageUpdateResponse>("chat.update", new Args
                {
                    { "ts", messageUpdate.Ts },
                    { "channel", messageUpdate.ChannelId },
                    { "text", messageUpdate.Text },
                    { "attachments", messageUpdate.Attachments },
                    { "blocks", messageUpdate.Blocks },
                    { "parse", messageUpdate.Parse },
                    { "link_names", messageUpdate.LinkNames },
                    { "as_user", messageUpdate.AsUser },
                    { "file_ids", messageUpdate.FileIds }
                },
            cancellationToken);

    public Task<MessageTsResponse> StartStream(string channel, string threadTs, string markdownText = null, string recipientUserId = null, string recipientTeamId = null, CancellationToken cancellationToken = default) =>
        client.Post<MessageTsResponse>("chat.startStream", new Args
            {
                { "channel", channel },
                { "thread_ts", threadTs },
                { "markdown_text", markdownText },
                { "recipient_user_id", recipientUserId },
                { "recipient_team_id", recipientTeamId }
            }, cancellationToken);

    public Task<MessageTsResponse> AppendStream(string channel, string ts, string markdownText, CancellationToken cancellationToken = default) =>
        client.Post<MessageTsResponse>("chat.appendStream", new Args
            {
                { "channel", channel },
                { "ts", ts },
                { "markdown_text", markdownText }
            }, cancellationToken);

    public Task<PostMessageResponse> StopStream(
        string channel,
        string ts,
        string markdownText = null,
        IEnumerable<Block> blocks = null,
        object metadataObject = null,
        MessageMetadata metadataJson = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<PostMessageResponse>("chat.stopStream", new Args
            {
                { "channel", channel },
                { "ts", ts },
                { "markdown_text", markdownText },
                { "blocks", blocks },
                { "metadata", metadataJson ?? MessageMetadata.FromObject(metadataObject, jsonSettings) }
            }, cancellationToken);
}
