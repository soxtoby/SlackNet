using System.Collections.Generic;

namespace SlackNet.WebApi
{
    public class FileListResponse
    {
        public IList<File> Files { get; set; } = new List<File>();
        public Paging Paging { get; set; } = new();
        public ResponseMetadata ResponseMetadata { get; set; } = new();
    }
}