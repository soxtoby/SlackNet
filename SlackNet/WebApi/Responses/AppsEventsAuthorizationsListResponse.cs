using System.Collections.Generic;

namespace SlackNet.WebApi
{
    public class AppsEventsAuthorizationsListResponse
    {
        public IList<Authorization> Authorizations { get; set; }  = new List<Authorization>();
        public ResponseMetadata ResponseMetadata { get; set; } = new ResponseMetadata();
    }
}