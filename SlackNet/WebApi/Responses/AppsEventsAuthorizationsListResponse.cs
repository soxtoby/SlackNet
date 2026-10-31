using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AppsEventsAuthorizationsListResponse : SlackObject
{
    public IList<Authorization> Authorizations { get; set; }  = [];
    public string CursorNext { get; set; }
}