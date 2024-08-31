using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlackNet.AspNetCore;

namespace SlackNet.AzureFunctions;

public class SlackFunctionResult(SlackResult slackResult) : IActionResult
{
    public async Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        
        response.StatusCode = (int)slackResult.Status;

        // Request service provider is disposed before OnCompleted callbacks are called,
        // so we'll just need to wait for them to run before responding
        foreach (var callback in slackResult.RequestCompletedCallbacks)
            await callback().ConfigureAwait(false);

        if (slackResult.ContentType != null)
            response.ContentType = slackResult.ContentType;

        if (slackResult.Body != null)
            await response.WriteAsync(slackResult.Body).ConfigureAwait(false);
    }
}

public static class SlackResultExtensions
{
    public static async Task<SlackFunctionResult> FunctionResult(this Task<SlackResult> slackResult) => (await slackResult.ConfigureAwait(false)).FunctionResult();
    public static SlackFunctionResult FunctionResult(this SlackResult slackResult) => new(slackResult);
}