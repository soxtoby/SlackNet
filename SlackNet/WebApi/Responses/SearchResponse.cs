using SlackNet.Events.Messages;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    public class SearchResponse
    {
        public string Query { get; set; }
        public SearchResults<Message> Messages { get; set; }
        public SearchResults<File> Files { get; set; }
    }
}