using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Events.Messages;
using SlackNet.Objects;
using SlackNet.WebApi;

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
        Message RawMessage { get; }
        Task Reply(string text, bool createThread = false);
        Task Reply(SlackMessage message, bool createThread = false);
    }
}