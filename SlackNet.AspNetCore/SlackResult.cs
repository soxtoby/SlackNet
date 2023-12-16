using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlackNet.AspNetCore;

public abstract class SlackResult
{
    private readonly List<Func<Task>> _requestCompletedCallbacks = new();

    protected SlackResult(HttpStatusCode status) => Status = status;

    public HttpStatusCode Status { get; }
    public IReadOnlyCollection<Func<Task>> RequestCompletedCallbacks => _requestCompletedCallbacks;

    /// <summary>
    /// Registers a callback to be called when the request ends.
    /// Callbacks will be called in first-in-first-out order.
    /// </summary>
    public SlackResult OnCompleted(Func<Task> callback)
    {
        _requestCompletedCallbacks.Add(callback);
        return this;
    }

    public virtual string ContentType => null;
    public virtual string Body => null;
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

    public override string ContentType => "text/plain";
    public override string Body { get; }
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

    public override string ContentType => "application/json";
    public override string Body => JsonConvert.SerializeObject(_data, _jsonSettings.SerializerSettings);
}