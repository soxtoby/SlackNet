using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet.WebApi;

public class ErrorResponse
{
    public string Error { get; set; }
    public ErrorResponseMetadata ResponseMetadata { get; set; }

    [JsonExtensionData]
    private readonly IDictionary<string, JToken> _info = new Dictionary<string, JToken>();
    
    public IReadOnlyDictionary<string, string> Info => _info.ToDictionary(e => e.Key, e => e.Value.ToObject<string>());
}