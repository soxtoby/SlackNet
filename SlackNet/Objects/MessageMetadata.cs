#nullable disable
using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet;

public class MessageMetadata
{
    public string EventType { get; set; }

    [JsonIgnore]
    [Obsolete("Use Payload instead.")]
    public JToken EventPayload
    {
        get => Payload.ToJToken();
        set => Payload = value.ToJsonData();
    }

    [JsonProperty("event_payload")]
    public JsonData Payload { get; set; }

    public static MessageMetadata FromObject(object payload, SlackJsonSettings slackJsonSettings = null) =>
        payload is not null
            ? new MessageMetadata
                {
                    EventType = payload.GetType().GetTypeInfo().SlackType(),
                    Payload = JsonData.FromObject(payload, slackJsonSettings)
                }
            : null;

    public T ToObject<T>(SlackJsonSettings slackJsonSettings = null) =>
        Payload.ToObject<T>(slackJsonSettings);
}
