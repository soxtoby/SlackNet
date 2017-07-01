namespace SlackNet.WebApi.Responses
{
    public class Paging
    {
        public int Count { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }
    }
}