using System.Collections.Generic;

namespace SlackNet.WebApi.Responses
{
    public class IntegrationLogsResponse
    {
        public IList<IntegrationLogRecord> Logs { get; set; }
        public Paging Paging { get; set; }
    }
}