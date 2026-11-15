#nullable enable
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SlackNet;

/// <summary>
/// Reads and writes properties that SlackNet doesn't model, kept in a <see cref="Newtonsoft.Json.JsonExtensionDataAttribute"/> dictionary.
/// </summary>
static class ExtensionData
{
    // Newtonsoft.Json reads primitive values into object-valued extension data as CLR values, and everything else as JTokens
    public static T? Read<T>(IDictionary<string, object?> extensionData, string propertyName, SlackJsonSettings? jsonSettings) =>
        extensionData.TryGetValue(propertyName, out var value)
            ? StoredToken(value).ToObject<T>(SlackJsonSettings.Serializer(jsonSettings))
            : default;

    public static JToken ToToken(object? value, SlackJsonSettings? jsonSettings) =>
        value is null
            ? JValue.CreateNull()
            : JToken.FromObject(value, SlackJsonSettings.Serializer(jsonSettings));

    private static JToken StoredToken(object? value) =>
        value switch
            {
                JToken token => token,
                null => JValue.CreateNull(),
                _ => JToken.FromObject(value)
            };
}
