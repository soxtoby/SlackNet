using System.Collections.Generic;
using SlackNet.Events.Messages;

namespace SlackNet.WebApi.Responses
{
    class RepliesResponse
    {
        public List<Message> Messages { get; } = new List<Message>();
    }
}