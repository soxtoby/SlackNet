#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class InformationBarrierListResponse
{
    public IList<InformationBarrier> Barriers { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
