using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using SlackNet.AspNetCore;

namespace SlackNet.AzureFunctionExample
{
    public class SlackEndpoints
    {
        private readonly ISlackRequestHandler _requestHandler;
        private readonly SlackEndpointConfiguration _endpointConfig;

        public SlackEndpoints(ISlackRequestHandler requestHandler, SlackEndpointConfiguration endpointConfig)
        {
            _requestHandler = requestHandler;
            _endpointConfig = endpointConfig;
        }

        [FunctionName("event")]
        public Task<SlackResult> Event([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) => 
            _requestHandler.HandleEventRequest(request, _endpointConfig);

        [FunctionName("action")]
        public Task<SlackResult> Action([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) => 
            _requestHandler.HandleActionRequest(request, _endpointConfig);

        [FunctionName("options")]
        public Task<SlackResult> Options([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) => 
            _requestHandler.HandleOptionsRequest(request, _endpointConfig);

        [FunctionName("command")]
        public Task<SlackResult> Command([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request) => 
            _requestHandler.HandleSlashCommandRequest(request, _endpointConfig);
    }
}
