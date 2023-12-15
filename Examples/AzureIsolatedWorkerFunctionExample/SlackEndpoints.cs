using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SlackNet.AzureFunctions;

namespace AzureIsolatedWorkerFunctionExample;

public class SlackEndpoints
{
    private readonly ISlackRequestHandler _requestHandler;
    private readonly SlackEndpointConfiguration _endpointConfig;

    public SlackEndpoints(ISlackRequestHandler requestHandler, SlackEndpointConfiguration endpointConfig)
    {
        _requestHandler = requestHandler;
        _endpointConfig = endpointConfig;
    }

    [Function("event")]
    public Task<SlackResult> Event([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request) => 
        _requestHandler.HandleEventRequest(request, _endpointConfig);

    [Function("action")]
    public Task<SlackResult> Action([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request) => 
        _requestHandler.HandleActionRequest(request, _endpointConfig);

    [Function("options")]
    public Task<SlackResult> Options([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request) => 
        _requestHandler.HandleOptionsRequest(request, _endpointConfig);

    [Function("command")]
    public Task<SlackResult> Command([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request) => 
        _requestHandler.HandleSlashCommandRequest(request, _endpointConfig);
}