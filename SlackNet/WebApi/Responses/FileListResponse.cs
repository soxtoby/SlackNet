using System.Collections.Generic;

namespace SlackNet.WebApi;

public class FileListResponse
{
    public IList<File> Files { get; set; } = [];
    public Paging Paging { get; set; } = new();
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}