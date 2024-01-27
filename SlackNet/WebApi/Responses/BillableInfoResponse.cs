using System.Collections.Generic;

namespace SlackNet.WebApi;

class BillableInfoResponse
{
    public IList<BillableInfo> BillableInfo { get; set; } = [];
}