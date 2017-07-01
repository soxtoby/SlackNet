using System.Collections.Generic;

namespace SlackNet.WebApi.Responses
{
    class BillableInfoResponse
    {
        public IList<BillableInfo> BillableInfo { get; } = new List<BillableInfo>();
    }
}