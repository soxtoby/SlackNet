#nullable enable
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SlackNet;

public class SlackJsonSettings
{
    public SlackJsonSettings(JsonSerializerSettings settings)
    {
        SerializerSettings = settings;

        // Lets deserialized objects remember these settings, to read the properties SlackNet doesn't model with them later.
        // Newtonsoft.Json still passes its context to callbacks as a StreamingContext, even though formatter-based serialization is obsolete.
#pragma warning disable SYSLIB0050
        if (settings.Context.Context is null)
            settings.Context = new StreamingContext(settings.Context.State, this);
#pragma warning restore SYSLIB0050
    }

    public JsonSerializerSettings SerializerSettings { get; }

    /// <summary>
    /// The settings an object is being deserialized with, from an <see cref="OnDeserializedAttribute"/> callback's context.
    /// </summary>
    internal static SlackJsonSettings? FromContext(StreamingContext context) => context.Context as SlackJsonSettings;

    internal static JsonSerializer Serializer(SlackJsonSettings? jsonSettings) =>
        JsonSerializer.Create((jsonSettings ?? Default.JsonSettings()).SerializerSettings);
}