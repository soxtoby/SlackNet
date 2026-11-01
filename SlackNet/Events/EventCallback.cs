using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.Events;

/// <summary>
/// Also referred to as the "outer event", or the JSON object containing the event that happened itself.
/// </summary>
public class EventCallback : EventRequest
{
    public string TeamId { get; set; }
    public string EnterpriseId { get; set; }
    /// <summary>The workspace through which your app is receiving the event (i.e., where the app is installed).</summary>
    public string ContextTeamId { get; set; }
    /// <summary>The enterprise org through which your app is receiving the event (i.e., where the app is installed).</summary>
    public string ContextEnterpriseId { get; set; }
    public string ApiAppId { get; set; }
    public bool IsExtSharedChannel { get; set; }
    public Event Event { get; set; }
    public IList<Authorization> Authorizations { get; set; }
    public string EventContext { get; set; }
    public string EventId { get; set; }
    public int EventTime { get; set; }
    [JsonIgnore]
    public DateTime EventDateTime => EventTime.ToDateTime().GetValueOrDefault();
}