using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SlackNet.AspNetCore
{
    class SlackRequestMiddleware
    {
        private readonly ISlackRequestHandler _requestHandler;
        private readonly SlackJsonSettings _jsonSettings;
        private readonly RequestDelegate _next;
        private readonly SlackEndpointConfiguration _configuration;

        public SlackRequestMiddleware(
            RequestDelegate next,
            SlackEndpointConfiguration configuration,
            ISlackRequestHandler requestHandler,
            SlackJsonSettings jsonSettings)
        {
            _next = next;
            _configuration = configuration;
            _requestHandler = requestHandler;
            _jsonSettings = jsonSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == $"/{_configuration.RoutePrefix}/event")
                await Respond(context.Response, await _requestHandler.HandleEventRequest(context.Request, _configuration).ConfigureAwait(false));
            else if (context.Request.Path == $"/{_configuration.RoutePrefix}/action")
                await Respond(context.Response, await _requestHandler.HandleActionRequest(context.Request, _configuration));
            else if (context.Request.Path == $"/{_configuration.RoutePrefix}/options") 
                await Respond(context.Response, await _requestHandler.HandleOptionsRequest(context.Request, _configuration));
            else
                await _next(context).ConfigureAwait(false);
        }

        private async Task Respond(HttpResponse httpResponse, SlackResponse slackResponse)
        {
            httpResponse.StatusCode = (int)slackResponse.Status;

            if (slackResponse.ContentType != null)
                httpResponse.ContentType = slackResponse.ContentType;

            if (slackResponse.Body(_jsonSettings) is string body)
                await httpResponse.WriteAsync(body).ConfigureAwait(false);
        }
    }
}