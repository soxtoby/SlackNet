using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet.WebApi;

public class ErrorResponse
{
    public string Warning { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public ErrorResponseMetadata ResponseMetadata { get; set; } = new();

    [JsonProperty] // Mapped so it isn't one of the fields
    private bool Ok { get; set; }

    [JsonExtensionData]
    private readonly IDictionary<string, JToken> _fields = new Dictionary<string, JToken>();

    /// <summary>
    /// The serializer that read this response, so fields can be read with the same JSON conventions.
    /// </summary>
    [JsonIgnore]
    internal JsonSerializer? Serializer { get; set; }

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
    /// Reads a field specific to the error, as returned by Slack, such as the items in a request that Slack couldn't process.
    /// </summary>
    /// <param name="fieldName">Name of the field, as returned by Slack.</param>
    /// <returns>The field's value, or the default value if Slack didn't return the field.</returns>
    public T? Field<T>(string fieldName) =>
        _fields.TryGetValue(fieldName, out var value)
            ? value.ToObject<T>(FieldSerializer)
            : default;

    /// <summary>
    /// A line for each field specific to the error, for exception messages.
    /// Error details are left out, since they're described separately.
    /// </summary>
    internal IEnumerable<string> FieldDescriptions =>
        _fields
            .Where(f => !IsShownAsErrorDetails(f))
            .Select(f => $"{f.Key}: {FieldText(f.Value)}");

    // Some methods return errors in other shapes, such as conversations.invite's per-user errors, which aren't error details
    private static bool IsErrorDetail(JToken error) =>
        error is JObject errorObject && errorObject.ContainsKey("message");

    private static bool IsShownAsErrorDetails(KeyValuePair<string, JToken> field) =>
        field is { Key: "errors", Value: JArray { Count: > 0 } errors }
        && errors.All(IsErrorDetail);

    private static string FieldText(JToken value) =>
        value.Type == JTokenType.String
            ? value.ToObject<string>()!
            : value.ToString(Formatting.None);

    private JsonSerializer FieldSerializer => Serializer ?? JsonSerializer.Create(Default.JsonSettings().SerializerSettings);
}
