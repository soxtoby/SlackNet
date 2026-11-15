using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet;

/// <summary>
/// The function execution that generated an interaction.
/// </summary>
public class FunctionData
{
    public string ExecutionId { get; set; }
    public Function Function { get; set; }

    [JsonIgnore]
    [Obsolete("Use InputValues instead.")]
    public JObject Inputs
    {
        get => (JObject)InputValues.ToJToken();
        set => InputValues = value.ToJsonData();
    }

    [JsonProperty("inputs")]
    public JsonData InputValues { get; set; } = new JObject().ToJsonData();
}
