using System.Collections.Generic;

namespace SlackNet.WebApi;

public class BookmarkListResponse : SlackObject
{
    public IList<Bookmark> Bookmarks { get; set; } = [];
}