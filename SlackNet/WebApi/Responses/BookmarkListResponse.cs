using System.Collections.Generic;

namespace SlackNet.WebApi;

public class BookmarkListResponse
{
    public IList<Bookmark> Bookmarks { get; set; } = [];
}