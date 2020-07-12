using System.Collections.Generic;

namespace SlackNet.WebApi
{
    public class FileAndCommentsResponse : FileResponse
    {
        public IList<FileComment> Comments { get; set; } = new List<FileComment>();
        public Paging Paging { get; set; } = new Paging();
        public ResponseMetadata ResponseMetadata { get; set; } = new ResponseMetadata();
    }
}