using System.Collections.Generic;

namespace SlackNet.WebApi;

class ManifestValidationResponse
{
    public IList<ErrorDetail> Warnings { get; set; } = [];
}
