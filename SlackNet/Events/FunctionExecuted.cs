using Newtonsoft.Json.Linq;

namespace SlackNet.Events;

public class FunctionExecuted<TInputs> : Event
{
    /// <summary>
    /// The function being executed.
    /// </summary>
    public Function Function { get; set; }

    public string FunctionExecutionId { get; set; }
    public string WorkflowExecutionId { get; set; }
    public string BotAccessToken { get; set; }
    public TInputs Inputs { get; set; }
}

public class FunctionExecuted : FunctionExecuted<JObject>
{
    public FunctionExecuted() => Inputs = new();
}
