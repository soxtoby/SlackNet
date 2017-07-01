namespace SlackNet.WebApi.Responses
{
    public class Pagination
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int PageCount { get; set; }
        public int First { get; set; }
        public int Last { get; set; }
    }
}