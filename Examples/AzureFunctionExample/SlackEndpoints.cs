using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SlackNet.AzureFunctions;

namespace AzureFunctionExample;

public class SlackEndpoints
{
    private readonly ISlackFunctionRequestHandler _requestHandler;
    public SlackEndpoints(ISlackFunctionRequestHandler requestHandler) => _requestHandler = requestHandler;

    [FunctionName("event")]
    public Task<SlackFunctionResult> Event([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) => 
        _requestHandler.HandleEventRequest(request);

    [FunctionName("action")]
    public Task<SlackFunctionResult> Action([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) => 
        _requestHandler.HandleActionRequest(request);

    [FunctionName("options")]
    public Task<SlackFunctionResult> Options([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) => 
        _requestHandler.HandleOptionsRequest(request);

    [FunctionName("command")]
    public Task<SlackFunctionResult> Command([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) => 
        _requestHandler.HandleSlashCommandRequest(request);
}