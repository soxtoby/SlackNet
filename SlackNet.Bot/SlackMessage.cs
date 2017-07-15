using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Events.Messages;
using SlackNet.Objects;
using SlackNet.WebApi;

namespace SlackNet.Bot
{
    public class SlackMessage : IMessage
    {
        private readonly SlackBot _bot;

        public SlackMessage(MessageEvent message, SlackBot bot)
        {
            RawMessage = message;
            _bot = bot;
        }

        public MessageEvent RawMessage { get; }
        public Hub Hub { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
        public string Ts { get; set; }
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
        public string ThreadTs { get; set; }
        public DateTime ThreadTimestamp => Ts.ToDateTime().GetValueOrDefault();
        public IList<Attachment> Attachments { get; set; } = new List<Attachment>();
        public bool IsInThread => ThreadTs != null;
        public bool MentionsBot => Text.IndexOf(_bot.Id, StringComparison.OrdinalIgnoreCase) >= 0
            || Text.IndexOf(_bot.Name, StringComparison.OrdinalIgnoreCase) >= 0
            || Hub.IsIm;

        public Task Reply(string text, bool createThread = false) => Reply(new Message { Text = text }, createThread);

        public async Task Reply(Func<Task<Message>> createReply, bool createThread = false)
        {
            await _bot.WhileTyping(Hub.Id, async () =>
                {
                    Message reply = await createReply().ConfigureAwait(false);
                    if (reply != null)
                        await Reply(reply, createThread).ConfigureAwait(false);
                }).ConfigureAwait(false);
        }

        public async Task Reply(Message message, bool createThread = false)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            message.AsUser = true;
            message.Channel = Hub.Id;
            if (ThreadTs != null)
                message.ThreadTs = ThreadTs;
            else if (createThread)
                message.ThreadTs = Ts;
            await _bot.Send(message).ConfigureAwait(false);
        }
    }
}