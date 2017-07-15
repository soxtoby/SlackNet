using System.Collections.Generic;
using SlackNet.Events.Messages;

namespace SlackNet.WebApi.Responses
{
    class RepliesResponse
    {
        public List<MessageEvent> Messages { get; } = new List<MessageEvent>();
    }
}