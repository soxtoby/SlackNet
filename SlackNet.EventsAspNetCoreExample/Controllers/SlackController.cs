using Microsoft.AspNetCore.Mvc;
using SlackNet.AspNetCore;
using SlackNet.EventsAspNetCoreExample.Models;

namespace SlackNet.EventsAspNetCoreExample.Controllers
{
    [ApiController]
    public class SlackController : ControllerBase
    {
        private readonly ISlackRequestHandler _requestHandler;
        private readonly SlackEndpointConfiguration _endpointConfig;
        private readonly ISlackApiClient _slack;
        public SlackController(ISlackRequestHandler requestHandler, SlackEndpointConfiguration endpointConfig, ISlackApiClient slack)
        {
            _requestHandler = requestHandler;
            _endpointConfig = endpointConfig;
            _slack = slack;
        }

        [HttpPost]
        [Route("[Controller]/Submit")]
        public async Task<ActionResult> Submit([FromBody] SlackRequest request)
        {
            await _slack.Chat.PostMessage(new SlackNet.WebApi.Message() { Text = request.Message, Channel = request.SlackChannel }, null);
            return Ok();
        }

        [HttpPost]
        [Route("[Controller]/Event")]
        public async Task<IActionResult> Event()
        {
            return await _requestHandler.HandleEventRequest(HttpContext.Request, _endpointConfig);
        }

    }
}