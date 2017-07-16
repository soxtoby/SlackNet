using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.Bot
{
    public class SlackMessage : IMessage
    {
        private readonly SlackBot _bot;

        public SlackMessage(MessageEvent message, SlackBot bot)
        {
            MessageEvent = message;
            _bot = bot;
        }

        public MessageEvent MessageEvent { get; }
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

        public Task Reply(string text, bool createThread = false) => Reply(new BotMessage { Text = text }, createThread);

        public async Task Reply(Func<Task<BotMessage>> createReply, bool createThread = false)
        {
            await _bot.WhileTyping(Hub.Id, async () =>
                {
                    BotMessage reply = await createReply().ConfigureAwait(false);
                    if (reply != null)
                        await Reply(reply, createThread).ConfigureAwait(false);
                }).ConfigureAwait(false);
        }

        public async Task Reply(BotMessage message, bool createThread = false)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            message.ReplyTo = this;
            message.CreateThread = createThread;
            await _bot.Send(message).ConfigureAwait(false);
        }
    }
}