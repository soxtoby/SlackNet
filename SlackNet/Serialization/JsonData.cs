#nullable enable
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet;

/// <summary>
/// JSON whose shape SlackNet doesn't define, such as data defined by your app.
/// Use <see cref="ToObject{T}"/> to read it into your own type, and <see cref="FromObject"/> to create it from your own type.
/// </summary>
[JsonConverter(typeof(JsonDataConverter))]
public sealed class JsonData
{
    private readonly JsonSerializer? _serializer;

    internal JsonData(JToken token, JsonSerializer? serializer)
    {
        Token = token;
        _serializer = serializer;
    }

    internal JToken Token { get; }

    /// <summary>
    /// Parses a JSON string.
    /// </summary>
    public static JsonData Parse(string json) => new(JToken.Parse(json), null);

    /// <summary>
    /// Converts an object to JSON using SlackNet's JSON conventions.
    /// </summary>
    /// <param name="value">The object to convert.</param>
    /// <param name="jsonSettings">The JSON settings to use, if not SlackNet's defaults.</param>
    public static JsonData FromObject(object value, SlackJsonSettings? jsonSettings = null)
    {
        var serializer = SlackJsonSettings.Serializer(jsonSettings);
        return new(JToken.FromObject(value, serializer), serializer);
    }

    /// <summary>
    /// Reads the JSON into an object using SlackNet's JSON conventions.
    /// </summary>
    /// <param name="jsonSettings">
    /// The JSON settings to use.
    /// Defaults to the settings the JSON was read or created with, or SlackNet's defaults.
    /// </param>
    public T? ToObject<T>(SlackJsonSettings? jsonSettings = null) =>
        Token.ToObject<T>(jsonSettings is null ? _serializer ?? SlackJsonSettings.Serializer(null) : SlackJsonSettings.Serializer(jsonSettings));

    /// <summary>
    /// The JSON as a string, without formatting.
    /// </summary>
    public override string ToString() => Token.ToString(Formatting.None);
}
