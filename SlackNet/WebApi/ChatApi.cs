using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IChatApi
    {
        /// <summary>
        /// Deletes a message from a channel.
        /// </summary>
        /// <param name="ts">Timestamp of the message to be deleted.</param>
        /// <param name="channelId">Channel containing the message to be deleted.</param>
        /// <param name="asUser">Pass True to delete the message as the authed user. Bot users in this context are considered authed users.</param>
        /// <param name="cancellationToken"></param>
        Task<MessageTsResponse> Delete(string ts, string channelId, bool asUser = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Sends a /me message to a channel from the calling user.
        /// </summary>
        /// <param name="channel">Channel to send message to. Can be a public channel, private group or IM channel. Can be an encoded ID, or a name.</param>
        /// <param name="text">Text of the message to send.</param>
        /// <param name="cancellationToken"></param>
        Task<MessageTsResponse> MeMessage(string channel, string text, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Posts a message to a public channel, private channel, or direct message/IM channel.
        /// </summary>
        /// <param name="message">The message to post</param>
        /// <param name="cancellationToken"></param>
        Task<PostMessageResponse> PostMessage(Message message, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Schedules a message for delivery to a public channel, private channel, or direct message/IM channel at a specified time in the future.
        /// </summary>
        /// <param name="message">The message to post.</param>
        /// <param name="postAt">Time in the future to send the message.</param>
        /// <param name="cancellationToken"></param>
        Task<ScheduleMessageResponse> ScheduleMessage(Message message, DateTime postAt, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Sends an ephemeral message to a user in a channel.
        /// </summary>
        /// <param name="userId">ID of the user who will receive the ephemeral message. The user should be in the channel specified by the channel argument.</param>
        /// <param name="message">The message to post. Not all message properties are supported by <c>PostEphemeral</c>.</param>
        /// <param name="cancellationToken"></param>
        Task<PostMessageResponse> PostEphemeral(string userId, Message message, CancellationToken? cancellationToken = null);
        
        /// <summary>
        /// Attaches Slack app unfurl behavior to a specified and relevant message. 
        /// A user token is required as this method does not support bot user tokens.
        /// </summary>
        /// <param name="channelId">Channel ID of the message.</param>
        /// <param name="ts">Timestamp of the message to add unfurl behavior to.</param>
        /// <param name="unfurls">Dictionary mapping a set of URLs from the message to their unfurl attachment.</param>
        /// <param name="userAuthRequired">Set to True to indicate the user must install your Slack app to trigger unfurls for this domain.</param>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// The first time this method is executed with a particular <see cref="ts"/> and <see cref="channelId"/> combination, 
        /// the valid <see cref="unfurls"/> attachments you provide will be attached to the message. 
        /// Subsequent attempts with the same <see cref="ts"/> and <see cref="channelId"/> values will modify the same attachments, rather than adding more.
        /// </remarks>
        Task Unfurl(string channelId, string ts, IDictionary<string, Attachment> unfurls, bool userAuthRequired = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Updates a message in a channel.
        /// </summary>
        /// <param name="messageUpdate">Message to update.</param>
        /// <param name="cancellationToken"></param>
        Task<MessageUpdateResponse> Update(MessageUpdate messageUpdate, CancellationToken? cancellationToken = null);
    }

    public class ChatApi : IChatApi
    {
        private readonly ISlackApiClient _client;
        public ChatApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Deletes a message from a channel.
        /// </summary>
        /// <param name="ts">Timestamp of the message to be deleted.</param>
        /// <param name="channelId">Channel containing the message to be deleted.</param>
        /// <param name="asUser">Pass True to delete the message as the authed user. Bot users in this context are considered authed users.</param>
        /// <param name="cancellationToken"></param>
        public Task<MessageTsResponse> Delete(string ts, string channelId, bool asUser = false, CancellationToken? cancellationToken = null) =>
            _client.Post<MessageTsResponse>("chat.delete", new Args
                {
                    { "ts", ts },
                    { "channel", channelId },
                    { "as_user", asUser }
                }, cancellationToken);

        /// <summary>
        /// Retrieve a permalink URL for a specific extant message.
        /// </summary>
        /// <param name="channelId">The ID of the conversation or channel containing the message.</param>
        /// <param name="messageTs">A message's timestamp, uniquely identifying it within a channel.</param>
        /// <param name="cancellationToken"></param>
        public Task<PermalinkResponse> GetPermalink(string channelId, string messageTs, CancellationToken? cancellationToken = null) =>
            _client.Get<PermalinkResponse>("chat.getPermalink", new Args
                {
                    { "channel", channelId },
                    { "message_ts", messageTs }
                }, cancellationToken);

        /// <summary>
        /// Sends a /me message to a channel from the calling user.
        /// </summary>
        /// <param name="channel">Channel to send message to. Can be a public channel, private group or IM channel. Can be an encoded ID, or a name.</param>
        /// <param name="text">Text of the message to send.</param>
        /// <param name="cancellationToken"></param>
        public Task<MessageTsResponse> MeMessage(string channel, string text, CancellationToken? cancellationToken = null) =>
            _client.Post<MessageTsResponse>("chat.meMessage", new Args
                {
                    { "channel", channel },
                    { "text", text }
                }, cancellationToken);

        /// <summary>
        /// Posts a message to a public channel, private channel, or direct message/IM channel.
        /// </summary>
        /// <param name="message">The message to post.</param>
        /// <param name="cancellationToken"></param>
        public Task<PostMessageResponse> PostMessage(Message message, CancellationToken? cancellationToken = null) =>
            _client.Post<PostMessageResponse>("chat.postMessage", PopulateMessageArgs(message, new Args()),
                cancellationToken);

        /// <summary>
        /// Schedules a message for delivery to a public channel, private channel, or direct message/IM channel at a specified time in the future.
        /// </summary>
        /// <param name="message">The message to post.</param>
        /// <param name="postAt">Time in the future to send the message.</param>
        /// <param name="cancellationToken"></param>
        public Task<ScheduleMessageResponse> ScheduleMessage(Message message, DateTime postAt, CancellationToken? cancellationToken = null) =>
            _client.Post<ScheduleMessageResponse>("chat.scheduleMessage", PopulateMessageArgs(message, new Args
                    {
                        { "post_at", postAt.ToTimestamp() }
                    }),
                cancellationToken);

        private static Args PopulateMessageArgs(Message message, Args args)
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
            return args;
        }

        /// <summary>
        /// Sends an ephemeral message to a user in a channel.
        /// </summary>
        /// <param name="userId">ID of the user who will receive the ephemeral message. The user should be in the channel specified by the channel argument.</param>
        /// <param name="message">The message to post. Not all message properties are supported by <c>PostEphemeral</c>.</param>
        /// <param name="cancellationToken"></param>
        public Task<PostMessageResponse> PostEphemeral(string userId, Message message, CancellationToken? cancellationToken = null) =>
            _client.Post<PostMessageResponse>("chat.postEphemeral", new Args
                    {
                        { "channel", message.Channel },
                        { "text", message.Text },
                        { "user", userId },
                        { "as_user", message.AsUser },
                        { "attachments", message.Attachments },
                        { "blocks", message.Blocks },
                        { "link_names", message.LinkNames },
                        { "parse", message.Parse }
                    },
                cancellationToken);

        /// <summary>
        /// Attaches Slack app unfurl behavior to a specified and relevant message. 
        /// A user token is required as this method does not support bot user tokens.
        /// </summary>
        /// <param name="channelId">Channel ID of the message.</param>
        /// <param name="ts">Timestamp of the message to add unfurl behavior to.</param>
        /// <param name="unfurls">Dictionary mapping a set of URLs from the message to their unfurl attachment.</param>
        /// <param name="userAuthRequired">Set to True to indicate the user must install your Slack app to trigger unfurls for this domain.</param>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// The first time this method is executed with a particular <see cref="ts"/> and <see cref="channelId"/> combination, 
        /// the valid <see cref="unfurls"/> attachments you provide will be attached to the message. 
        /// Subsequent attempts with the same <see cref="ts"/> and <see cref="channelId"/> values will modify the same attachments, rather than adding more.
        /// </remarks>
        public Task Unfurl(string channelId, string ts, IDictionary<string, Attachment> unfurls, bool userAuthRequired = false, CancellationToken? cancellationToken = null) =>
            _client.Post("chat.unfurl", new Args
                {
                    { "channel", channelId },
                    { "ts", ts },
                    { "unfurls", unfurls },
                    { "user_auth_required", userAuthRequired }
                }, cancellationToken);

        /// <summary>
        /// Updates a message in a channel.
        /// </summary>
        /// <param name="messageUpdate">Message to update.</param>
        /// <param name="cancellationToken"></param>
        public Task<MessageUpdateResponse> Update(MessageUpdate messageUpdate, CancellationToken? cancellationToken = null) =>
            _client.Post<MessageUpdateResponse>("chat.update", new Args
                    {
                        { "ts", messageUpdate.Ts },
                        { "channel", messageUpdate.ChannelId },
                        { "text", messageUpdate.Text },
                        { "attachments", messageUpdate.Attachments },
                        { "blocks", messageUpdate.Blocks },
                        { "parse", messageUpdate.Parse },
                        { "link_names", messageUpdate.LinkNames },
                        { "as_user", messageUpdate.AsUser }
                    },
                cancellationToken);
    }
}