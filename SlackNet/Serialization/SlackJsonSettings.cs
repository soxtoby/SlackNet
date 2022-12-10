using Newtonsoft.Json;

namespace SlackNet;

public class SlackJsonSettings
{
    public SlackJsonSettings(JsonSerializerSettings settings) => SerializerSettings = settings;

    public JsonSerializerSettings SerializerSettings { get; }
}