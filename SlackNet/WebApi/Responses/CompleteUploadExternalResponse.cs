using System.Collections.Generic;

namespace SlackNet.WebApi;

public class CompleteUploadExternalResponse
{
    public IList<ExternalFileReference> Files { get; set; } = [];
}