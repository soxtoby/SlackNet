using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using SlackNet.Events;

namespace SlackNet;

/// <summary>
/// Slack sends some events as their own requests instead of in an <see cref="EventCallback"/>,
/// so they're wrapped in one to reach event handlers like any other event.
/// </summary>
static class UnwrappedEvents
{
    private static readonly string AppRateLimitedType = typeof(AppRateLimited).GetTypeInfo().SlackType();
    private static readonly string EventCallbackType = typeof(EventCallback).GetTypeInfo().SlackType();

    public static JObject WrapInEventCallback(Type expectedType, JObject json) =>
        (expectedType == typeof(EventRequest) || expectedType == typeof(EventCallback))
        && json.Value<string>("type") == AppRateLimitedType
            ? new JObject
                {
                    ["type"] = EventCallbackType,
                    ["token"] = json["token"],
                    ["team_id"] = json["team_id"],
                    ["api_app_id"] = json["api_app_id"],
                    ["event"] = json
                }
            : json;
}
