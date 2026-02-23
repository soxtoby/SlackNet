using Newtonsoft.Json.Linq;

namespace SlackNet;

/// <summary>
/// The function execution that generated an interaction.
/// </summary>
public class FunctionData
{
    public string ExecutionId { get; set; }
    public Function Function { get; set; }
    public JObject Inputs { get; set; } = new();
}
