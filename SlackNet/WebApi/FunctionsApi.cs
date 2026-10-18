#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IFunctionsApi
{
    /// <summary>
    /// Signal that a function failed to complete.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/functions.completeError">Slack documentation</a> for more information.</remarks>
    /// <param name="functionExecutionId">Context identifier that maps to the executed function.</param>
    /// <param name="error">A human-readable error message that contains information about why the function failed to complete.</param>
    /// <param name="cancellationToken"></param>
    Task CompleteError(string functionExecutionId, string error, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signal the successful completion of a function.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/functions.completeSuccess">Slack documentation</a> for more information.</remarks>
    /// <param name="functionExecutionId">Context identifier that maps to the executed function.</param>
    /// <param name="outputs">A JSON-based object that conforms to the output parameters schema for the custom function defined in the manifest.</param>
    /// <param name="cancellationToken"></param>
    Task CompleteSuccess(string functionExecutionId, JObject outputs, CancellationToken cancellationToken = default);
}

public class FunctionsApi(ISlackApiClient client) : IFunctionsApi
{
    public Task CompleteError(string functionExecutionId, string error, CancellationToken cancellationToken = default) =>
        client.Post("functions.completeError", new Args
            {
                { "function_execution_id", functionExecutionId },
                { "error", error }
            }, cancellationToken);

    public Task CompleteSuccess(string functionExecutionId, JObject outputs, CancellationToken cancellationToken = default) =>
        client.Post("functions.completeSuccess", new Args
            {
                { "function_execution_id", functionExecutionId },
                { "outputs", outputs }
            }, cancellationToken);
}