using System.Collections.Generic;

namespace SlackNet.WebApi;

public class CompleteUploadExternalResponse : SlackObject
{
    public IList<ExternalFileReference> Files { get; set; } = [];
}