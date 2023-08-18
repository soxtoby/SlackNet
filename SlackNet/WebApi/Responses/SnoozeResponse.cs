using System;
using Newtonsoft.Json;

namespace SlackNet.WebApi;

public class SnoozeResponse
{
    public bool SnoozeEnabled { get; set; }
    public int SnoozeEndtime { get; set; }
    [JsonIgnore]
    public DateTime? SnoozeEnd => SnoozeEndtime.ToDateTime();
    public int SnoozeRemaining { get; set; }
    public TimeSpan SnoozeRemainingTimeSpan => TimeSpan.FromSeconds(SnoozeRemaining);
    public bool SnoozeIsIndefinite { get; set; }
}