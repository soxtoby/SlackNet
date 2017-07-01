using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    public class FileSearchResponse
    {
        public string Query { get; set; }
        public SearchResults<File> Files { get; set; }
    }
}