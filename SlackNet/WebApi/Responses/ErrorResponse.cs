#nullable enable
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

    [JsonExtensionData]
    private readonly IDictionary<string, JToken> _info = new Dictionary<string, JToken>();
    
    public IReadOnlyDictionary<string, string> Info => _info
        .Where(i => i.Value.Type == JTokenType.String)
        .ToDictionary(e => e.Key, e => e.Value.ToObject<string>()!);
}