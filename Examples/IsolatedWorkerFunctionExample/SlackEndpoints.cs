using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using SlackNet.AzureFunctions;

namespace IsolatedWorkerFunctionExample;

public class SlackEndpoints(ISlackFunctionRequestHandler requestHandler)
{
    [Function("event")]
    public Task<SlackFunctionResult> Event([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) => 
        requestHandler.HandleEventRequest(request);

    [Function("action")]
    public Task<SlackFunctionResult> Action([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) => 
        requestHandler.HandleActionRequest(request);

    [Function("options")]
    public Task<SlackFunctionResult> Options([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) =>
        requestHandler.HandleOptionsRequest(request);

    [Function("command")]
    public Task<SlackFunctionResult> Command([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) =>
        requestHandler.HandleSlashCommandRequest(request);
}