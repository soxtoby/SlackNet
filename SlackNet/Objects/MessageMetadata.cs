using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet;

public class MessageMetadata
{
    public string EventType { get; set; }
    
    public JToken EventPayload { get; set; }

    public static MessageMetadata FromObject(object payload, SlackJsonSettings slackJsonSettings = null) =>
        payload is not null
            ? new MessageMetadata
                {
                    EventType = payload.GetType().GetTypeInfo().SlackType(),
                    EventPayload = JToken.FromObject(payload, JsonSerializer.Create((slackJsonSettings ?? Default.JsonSettings()).SerializerSettings))
                }
            : null;

    public T ToObject<T>(SlackJsonSettings slackJsonSettings = null) =>
        EventPayload.ToObject<T>(
            JsonSerializer.Create((slackJsonSettings ?? Default.JsonSettings()).SerializerSettings));
}