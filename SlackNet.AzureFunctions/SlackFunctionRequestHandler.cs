using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SlackNet.AspNetCore;

namespace SlackNet.AzureFunctions;

public interface ISlackFunctionRequestHandler
{
    Task<SlackFunctionResult> HandleEventRequest(HttpRequest request);
    Task<SlackFunctionResult> HandleActionRequest(HttpRequest request);
    Task<SlackFunctionResult> HandleOptionsRequest(HttpRequest request);
    Task<SlackFunctionResult> HandleSlashCommandRequest(HttpRequest request);
}

class SlackFunctionRequestHandler(ISlackRequestHandler requestHandler) : ISlackFunctionRequestHandler
{
    public Task<SlackFunctionResult> HandleEventRequest(HttpRequest request) =>
        requestHandler.HandleEventRequest(request).FunctionResult();

    public Task<SlackFunctionResult> HandleActionRequest(HttpRequest request) =>
        requestHandler.HandleActionRequest(request).FunctionResult();

    public Task<SlackFunctionResult> HandleOptionsRequest(HttpRequest request) =>
        requestHandler.HandleOptionsRequest(request).FunctionResult();

    public Task<SlackFunctionResult> HandleSlashCommandRequest(HttpRequest request) =>
        requestHandler.HandleSlashCommandRequest(request).FunctionResult();
}