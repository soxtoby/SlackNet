using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using SlackNet.AspNetCore;

namespace SlackNet.AzureFunctionExample
{
    public class SlackEndpoints
    {
        private readonly ISlackRequestHandler _requestHandler;
        private readonly SlackJsonSettings _jsonSettings;
        private readonly SlackEndpointConfiguration _endpointConfig;

        public SlackEndpoints(ISlackRequestHandler requestHandler, SlackJsonSettings jsonSettings, SlackEndpointConfiguration endpointConfig)
        {
            _requestHandler = requestHandler;
            _jsonSettings = jsonSettings;
            _endpointConfig = endpointConfig;
        }

        [FunctionName("event")]
        public async Task<IActionResult> Event([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
        {
            return SlackResponse(await _requestHandler.HandleEventRequest(request, _endpointConfig).ConfigureAwait(false));
        }

        [FunctionName("action")]
        public async Task<IActionResult> Action([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
        {
            return SlackResponse(await _requestHandler.HandleActionRequest(request, _endpointConfig).ConfigureAwait(false));
        }

        [FunctionName("options")]
        public async Task<IActionResult> Options([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
        {
            return SlackResponse(await _requestHandler.HandleOptionsRequest(request, _endpointConfig).ConfigureAwait(false));
        }

        private ContentResult SlackResponse(SlackResponse response)
        {
            return new ContentResult
                {
                    StatusCode = (int)response.Status,
                    ContentType = response.ContentType,
                    Content = response.Body(_jsonSettings)
                };
        }
    }
}
