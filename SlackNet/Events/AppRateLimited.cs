#nullable disable
using System;
using Newtonsoft.Json;

namespace SlackNet.Events;

/// <summary>
/// Indicates your app's event subscriptions are being rate limited.
/// This event type is only dispatched when your app is rate limited on the Events API.
/// </summary>
/// <remarks>
/// <para>This Events API-only event type has no "inner event".
/// SlackNet passes it to event handlers in an <see cref="EventCallback"/> with the same <see cref="EventCallback.Token"/>, <see cref="EventCallback.TeamId"/> and <see cref="EventCallback.ApiAppId"/>.</para>
/// <para>See the <a href="https://docs.slack.dev/reference/events/app_rate_limited/">Slack documentation</a> for more information.</para>
/// </remarks>
public class AppRateLimited : Event
{
    /// <summary>
    /// The same shared token that once was used to verify other events in the Events API.
    /// This is deprecated in favor of signed secrets.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Subscriptions between your app and the workspace with this ID are being rate limited.
    /// </summary>
    public string TeamId { get; set; }

    /// <summary>
    /// A rounded epoch time value indicating the minute your application became rate limited for this workspace.
    /// </summary>
    public int MinuteRateLimited { get; set; }

    /// <summary>
    /// The minute your application became rate limited for this workspace.
    /// </summary>
    [JsonIgnore]
    public DateTime RateLimitedDateTime => MinuteRateLimited.ToDateTime().GetValueOrDefault();

    /// <summary>
    /// Your application's ID, especially useful if you have multiple applications working with the Events API.
    /// </summary>
    public string ApiAppId { get; set; }
}
