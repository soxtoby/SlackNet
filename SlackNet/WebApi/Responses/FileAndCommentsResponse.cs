using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    public class FileAndCommentsResponse : FileResponse
    {
        public IList<FileComment> Comments { get; set; } = new List<FileComment>();
        public Paging Paging { get; set; }
    }
}