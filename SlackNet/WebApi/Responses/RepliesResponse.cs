using System.Collections.Generic;
using SlackNet.Events;

namespace SlackNet.WebApi
{
    class RepliesResponse
    {
        public List<MessageEvent> Messages { get; } = new List<MessageEvent>();
    }
}