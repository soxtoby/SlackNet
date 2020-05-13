using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SlackNet.AspNetCore
{
    class SlackRequestMiddleware
    {
        private readonly ISlackRequestHandler _requestHandler;
        private readonly RequestDelegate _next;
        private readonly SlackEndpointConfiguration _configuration;

        public SlackRequestMiddleware(
            RequestDelegate next,
            SlackEndpointConfiguration configuration,
            ISlackRequestHandler requestHandler)
        {
            _next = next;
            _configuration = configuration;
            _requestHandler = requestHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == $"/{_configuration.RoutePrefix}/event")
                await Respond(context.Response, await _requestHandler.HandleEventRequest(context.Request, _configuration).ConfigureAwait(false)).ConfigureAwait(false);
            else if (context.Request.Path == $"/{_configuration.RoutePrefix}/action")
                await Respond(context.Response, await _requestHandler.HandleActionRequest(context.Request, _configuration).ConfigureAwait(false)).ConfigureAwait(false);
            else if (context.Request.Path == $"/{_configuration.RoutePrefix}/options") 
                await Respond(context.Response, await _requestHandler.HandleOptionsRequest(context.Request, _configuration).ConfigureAwait(false)).ConfigureAwait(false);
            else if (context.Request.Path == $"/{_configuration.RoutePrefix}/command")
                await Respond(context.Response, await _requestHandler.HandleSlashCommandRequest(context.Request, _configuration).ConfigureAwait(false)).ConfigureAwait(false);
            else
                await _next(context).ConfigureAwait(false);
        }

        private static Task Respond(HttpResponse httpResponse, SlackResult slackResult) => slackResult.ExecuteResultAsync(httpResponse);
    }
}