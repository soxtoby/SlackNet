using System;

namespace SlackNet.WebApi
{
    public class IntegrationLogRecord
    {
        public string ServiceId { get; set; }
        public string ServiceType { get; set; }
        public string AppId { get; set; }
        public string AppType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Channel { get; set; }
        public string Date { get; set; }
        public DateTime DateTime => Date.ToDateTime().GetValueOrDefault();
        public ChangeType ChangeType { get; set; }
        public string Scope { get; set; }
        public bool RssFeed { get; set; }
        public string RssFeedChangeType { get; set; }
        public string RssFeedTitle { get; set; }
        public string RssFeedUrl { get; set; }
        public DisabledReason Reason { get; set; }
    }
}