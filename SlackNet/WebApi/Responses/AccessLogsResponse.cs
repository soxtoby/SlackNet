using System.Collections.Generic;

namespace SlackNet.WebApi.Responses
{
    public class AccessLogsResponse
    {
        public IList<Login> Logins { get; set; } = new List<Login>();
        public Paging Paging { get; set; }
    }
}