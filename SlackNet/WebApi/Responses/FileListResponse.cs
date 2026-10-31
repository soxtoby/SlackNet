using System.Collections.Generic;

namespace SlackNet.WebApi;

public class FileListResponse : SlackObject
{
    public IList<File> Files { get; set; } = [];
    public Paging Paging { get; set; } = new();
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}