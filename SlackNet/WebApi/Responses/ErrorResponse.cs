#nullable enable
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet.WebApi;

public class ErrorResponse
{
    public string Warning { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public ErrorResponseMetadata ResponseMetadata { get; set; } = new();

    [JsonProperty] // Mapped so it isn't one of the Fields
    private bool Ok { get; set; }

    [JsonExtensionData]
    private readonly IDictionary<string, JToken> _fields = new Dictionary<string, JToken>();

    /// <summary>
    /// The serializer that read this response, so fields can be read with the same JSON conventions.
    /// </summary>
    [JsonIgnore]
    internal JsonSerializer? Serializer { get; set; }

    /// <summary>
    /// Fields specific to the error, as returned by Slack, such as the items in a request that Slack couldn't process.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyDictionary<string, JToken> Fields => new ReadOnlyDictionary<string, JToken>(_fields);

    [JsonIgnore]
    public IReadOnlyDictionary<string, string> Info => _fields
        .Where(i => i.Value.Type == JTokenType.String)
        .ToDictionary(e => e.Key, e => e.Value.ToObject<string>()!);

    /// <summary>
    /// Issues found in the request, each with a JSON pointer path to the invalid element.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyList<ErrorDetail> Errors =>
        _fields.TryGetValue("errors", out var errors) && errors is JArray
            ? errors
                .Where(IsErrorDetail)
                .Select(error => error.ToObject<ErrorDetail>(FieldSerializer)!)
                .ToList()
            : [];

    /// <summary>
    /// Reads one of the <see cref="Fields"/> using SlackNet's JSON conventions.
    /// </summary>
    /// <param name="fieldName">Name of the field, as returned by Slack.</param>
    /// <returns>The field's value, or the default value if Slack didn't return the field.</returns>
    public T? Field<T>(string fieldName) =>
        _fields.TryGetValue(fieldName, out var value)
            ? value.ToObject<T>(FieldSerializer)
            : default;

    // Some methods return errors in other shapes, such as conversations.invite's per-user errors, which aren't error details
    internal static bool IsErrorDetail(JToken error) =>
        error is JObject errorObject && errorObject.ContainsKey("message");

    private JsonSerializer FieldSerializer => Serializer ?? JsonSerializer.Create(Default.JsonSettings().SerializerSettings);
}
