using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackNet.Blocks;

namespace SlackNet.Bot;

public class SlackMessage(ISlackBot bot) : IMessage
{
    public Conversation Conversation { get; set; }
    [Obsolete("Use Conversation instead")]
    public Hub Hub { get; set; }
    public User User { get; set; }
    public string Text { get; set; }
    public string Ts { get; set; }
    [JsonIgnore]
    public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
    public string ThreadTs { get; set; }
    [JsonIgnore]
    public DateTime ThreadTimestamp => Ts.ToDateTime().GetValueOrDefault();
    public IList<Attachment> Attachments { get; set; } = [];

    public IList<File> Files { get; set; } = [];

    public IList<Block> Blocks { get; set; } = [];
    public bool IsInThread => ThreadTs != null;

    public bool MentionsBot =>
        Text.IndexOf(bot.Id, StringComparison.OrdinalIgnoreCase) >= 0
        || Text.IndexOf(bot.Name, StringComparison.OrdinalIgnoreCase) >= 0
        || Conversation?.IsIm == true;

    public Task ReplyWith(string text, bool createThread = false, CancellationToken cancellationToken = default) =>
        ReplyWith(new BotMessage { Text = text }, createThread, cancellationToken);

    public async Task ReplyWith(Func<Task<BotMessage>> createReply, bool createThread = false, CancellationToken cancellationToken = default)
    {
        await bot.WhileTyping(Conversation.Id, async () =>
            {
                var reply = await createReply().ConfigureAwait(false);
                if (reply != null)
                    await ReplyWith(reply, createThread, cancellationToken).ConfigureAwait(false);
            }).ConfigureAwait(false);
    }

    public async Task ReplyWith(BotMessage message, bool createThread = false, CancellationToken cancellationToken = default)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        message.ReplyTo = this;
        message.CreateThread = createThread;
        await bot.Send(message, cancellationToken).ConfigureAwait(false);
    }
}