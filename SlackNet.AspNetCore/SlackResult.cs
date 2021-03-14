using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SlackNet.Handlers;

namespace SlackNet.AspNetCore
{
    public abstract class SlackResult : IActionResult
    {
        private readonly ISlackRequestListener _requestListener;
        private readonly SlackRequestContext _requestContext;
        private readonly HttpStatusCode _status;
        private Func<Task> _requestCompletedCallback = () => Task.CompletedTask;

        protected SlackResult(ISlackRequestListener requestListener, SlackRequestContext requestContext, HttpStatusCode status)
        {
            _requestListener = requestListener;
            _requestContext = requestContext;
            _status = status;
        }

        public SlackResult OnCompleted(Func<Task> callback)
        {
            _requestCompletedCallback = callback;
            return this;
        }

        public Task ExecuteResultAsync(ActionContext context) => ExecuteResultAsync(context.HttpContext.Response);

        public async Task ExecuteResultAsync(HttpResponse response)
        {
            response.StatusCode = (int)_status;

            // Note: completed callbacks are called last -> first
            response.OnCompleted(() => _requestListener.OnRequestEnd(_requestContext));
            response.OnCompleted(_requestCompletedCallback);

            if (ContentType != null)
                response.ContentType = ContentType;

            if (Body != null)
                await response.WriteAsync(Body).ConfigureAwait(false);
        }

        protected virtual string ContentType => null;
        protected virtual string Body => null;
    }

    class EmptyResult : SlackResult
    {
        public EmptyResult(ISlackRequestListener requestListener, SlackRequestContext requestContext, HttpStatusCode status)
            : base(requestListener, requestContext, status) { }
    }

    class StringResult : SlackResult
    {
        public StringResult(ISlackRequestListener requestListener, SlackRequestContext requestContext, HttpStatusCode status, string body)
            : base(requestListener, requestContext, status) => Body = body;

        protected override string Body { get; }
    }

    class JsonResult : SlackResult
    {
        private readonly SlackJsonSettings _jsonSettings;
        private readonly object _data;

        public JsonResult(ISlackRequestListener requestListener, SlackRequestContext requestContext, SlackJsonSettings jsonSettings, HttpStatusCode status, object data)
            : base(requestListener, requestContext, status)
        {
            _jsonSettings = jsonSettings;
            _data = data;
        }

        protected override string ContentType => "application/json";
        protected override string Body => JsonConvert.SerializeObject(_data, _jsonSettings.SerializerSettings);
    }
}