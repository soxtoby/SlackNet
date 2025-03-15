#nullable enable
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace SlackNet.Tests;

class TestWebSocket(string uri) : IWebSocket
{
    private readonly TaskCompletionSource _closed = new();
    private readonly Subject<string> _messages = new();

    public string Uri { get; } = uri;
    public List<string> Sent { get; } = new();
    
    public TaskCompletionSource<bool> Connection { get; } = new();

    public async Task<bool> Open(CancellationToken cancellationToken)
    {
        State = WebSocketState.Connecting;
        cancellationToken.Register(Connection.SetCanceled);
        var connected = await Connection.Task;
        State = connected ? WebSocketState.Open : WebSocketState.Closed;
        return connected;
    }

    public async Task Send(string message) => Sent.Add(message);

    public void Receive(string message) => _messages.OnNext(message);

    public WebSocketState State { get; private set; }
    public Task Closed => _closed.Task;
    public IObservable<string> Messages => _messages;

    public void Dispose()
    {
        State = WebSocketState.Closed;
        _closed.TrySetResult();
    }
}