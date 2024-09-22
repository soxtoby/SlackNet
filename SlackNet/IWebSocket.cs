using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;

namespace SlackNet;

public interface IWebSocket : IDisposable
{
    Task<bool> Open(CancellationToken cancellationToken);
    void Send(string message);
    WebSocketState State { get; }
    Task Closed { get; }
    IObservable<string> Messages { get; }
}

public class WebSocketWrapper(WebSocket webSocket) : IWebSocket
{
    private readonly TaskCompletionSource<int> _closed = new();
    
    public async Task<bool> Open(CancellationToken cancellationToken)
    {
        cancellationToken.Register(webSocket.Close);
        var success = await webSocket.OpenAsync().ConfigureAwait(false);
        if (success)
            webSocket.Closed += (_, _) => _closed.TrySetResult(0);
        return success;
    }

    public void Send(string message) => webSocket.Send(message);

    public WebSocketState State => webSocket.State;

    public Task Closed => _closed.Task;

    public IObservable<string> Messages => Observable.FromEventPattern<MessageReceivedEventArgs>(h => webSocket.MessageReceived += h, h => webSocket.MessageReceived -= h)
        .Select(e => e.EventArgs.Message);

    public void Dispose() => webSocket.Dispose();
}