using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SlackNet.AspNetCore
{
    public abstract class SlackResult : IActionResult
    {
        private readonly IList<Func<Task>> _requestCompletedCallbacks = new List<Func<Task>>();
        private readonly HttpStatusCode _status;

        protected SlackResult(HttpStatusCode status) => _status = status;

        /// <summary>
        /// Registers a callback to be called when the request ends.
        /// Callbacks will be called in first-in-first-out order.
        /// </summary>
        public SlackResult OnCompleted(Func<Task> callback)
        {
            _requestCompletedCallbacks.Add(callback);
            return this;
        }

        public Task ExecuteResultAsync(ActionContext context) => ExecuteResultAsync(context.HttpContext.Response);

        public async Task ExecuteResultAsync(HttpResponse response)
        {
            response.StatusCode = (int)_status;

            // Note: HttpResponse's completed callbacks are called in FILO order
            foreach (var callback in _requestCompletedCallbacks.Reverse())
                response.OnCompleted(callback);

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
        public EmptyResult(HttpStatusCode status)
            : base(status) { }
    }

    class StringResult : SlackResult
    {
        public StringResult(HttpStatusCode status, string body)
            : base(status) => Body = body;

        protected override string Body { get; }
    }

    class JsonResult : SlackResult
    {
        private readonly SlackJsonSettings _jsonSettings;
        private readonly object _data;

        public JsonResult(SlackJsonSettings jsonSettings, HttpStatusCode status, object data)
            : base(status)
        {
            _jsonSettings = jsonSettings;
            _data = data;
        }

        protected override string ContentType => "application/json";
        protected override string Body => JsonConvert.SerializeObject(_data, _jsonSettings.SerializerSettings);
    }
}