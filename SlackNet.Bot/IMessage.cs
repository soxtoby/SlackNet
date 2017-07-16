using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlackNet.Bot
{
    public interface IMessage
    {
        Hub Hub { get; set; }
        User User { get; set; }
        string Text { get; set; }
        string Ts { get; set; }
        DateTime Timestamp { get; }
        string ThreadTs { get; set; }
        DateTime ThreadTimestamp { get; }
        IList<Attachment> Attachments { get; set; }
        bool IsInThread { get; }
        bool MentionsBot { get; }
        Task Reply(string text, bool createThread = false);
        Task Reply(BotMessage message, bool createThread = false);
        Task Reply(Func<Task<BotMessage>> createReply, bool createThread = false);
    }
}