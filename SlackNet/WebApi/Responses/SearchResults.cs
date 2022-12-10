using System.Collections.Generic;

namespace SlackNet.WebApi;

public class SearchResults<T>
{
    public int Total { get; set; }
    public Pagination Pagination { get; set; }
    public Paging Paging { get; set; }
    public IList<T> Matches { get; set; } = new List<T>();
}