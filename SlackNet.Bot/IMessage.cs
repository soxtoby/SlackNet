using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Blocks;

namespace SlackNet.Bot
{
    public interface IMessage
    {
        Conversation Conversation { get; set; }
        [Obsolete("Use Conversation instead")]
        Hub Hub { get; set; }
        User User { get; set; }
        string Text { get; set; }
        string Ts { get; set; }
        DateTime Timestamp { get; }
        string ThreadTs { get; set; }
        DateTime ThreadTimestamp { get; }
        IList<Attachment> Attachments { get; set; }
        IList<Block> Blocks { get; set; }
        bool IsInThread { get; }
        bool MentionsBot { get; }
        Task ReplyWith(string text, bool createThread = false, CancellationToken? cancellationToken = null);
        Task ReplyWith(BotMessage message, bool createThread = false, CancellationToken? cancellationToken = null);
        Task ReplyWith(Func<Task<BotMessage>> createReply, bool createThread = false, CancellationToken? cancellationToken = null);
    }
}