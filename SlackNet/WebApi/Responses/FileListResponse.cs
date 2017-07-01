using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    public class FileListResponse
    {
        public IList<File> Files { get; set; } = new List<File>();
        public Paging Paging { get; set; }
    }
}