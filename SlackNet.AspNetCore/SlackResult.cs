using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SlackNet.AspNetCore
{
    public abstract class SlackResult : IActionResult
    {
        private readonly HttpStatusCode _status;
        private Func<Task> _requestCompletedCallback = () => Task.CompletedTask;

        protected SlackResult(HttpStatusCode status) => _status = status;

        public SlackResult OnCompleted(Func<Task> callback)
        {
            _requestCompletedCallback = callback;
            return this;
        }

        public Task ExecuteResultAsync(ActionContext context) => ExecuteResultAsync(context.HttpContext.Response);

        public async Task ExecuteResultAsync(HttpResponse response)
        {
            response.StatusCode = (int)_status;
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
        public EmptyResult(HttpStatusCode status) : base(status) { }
    }

    class StringResult : SlackResult
    {
        public StringResult(HttpStatusCode status, string body) : base(status) => Body = body;

        protected override string Body { get; }
    }

    class JsonResult : SlackResult
    {
        private readonly SlackJsonSettings _jsonSettings;
        private readonly object _data;

        public JsonResult(SlackJsonSettings jsonSettings, HttpStatusCode status, object data) : base(status)
        {
            _jsonSettings = jsonSettings;
            _data = data;
        }

        protected override string ContentType => "application/json";
        protected override string Body => JsonConvert.SerializeObject(_data, _jsonSettings.SerializerSettings);
    }
}