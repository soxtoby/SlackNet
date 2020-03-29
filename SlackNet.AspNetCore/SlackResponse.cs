using System.Net;
using Newtonsoft.Json;

namespace SlackNet.AspNetCore
{
    public abstract class SlackResponse
    {
        protected SlackResponse(HttpStatusCode status)
        {
            Status = status;
        }

        public HttpStatusCode Status { get; }
        public virtual string ContentType => null;
        public virtual string Body(SlackJsonSettings jsonSettings) => null;
    }

    class EmptyResponse : SlackResponse
    {
        public EmptyResponse(HttpStatusCode status) : base(status) { }
    }

    class StringResponse : SlackResponse
    {
        private readonly string _body;
        public StringResponse(HttpStatusCode status, string body) : base(status) => _body = body;
        
        public override string Body(SlackJsonSettings jsonSettings) => _body;
    }

    class JsonResponse : SlackResponse
    {
        private readonly object _data;
        public JsonResponse(HttpStatusCode status, object data) : base(status) => _data = data;

        public override string ContentType => "application/json";
        public override string Body(SlackJsonSettings jsonSettings) => JsonConvert.SerializeObject(_data, jsonSettings.SerializerSettings);
    }
}