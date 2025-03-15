using System;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlackNet;

public interface IWebSocket : IDisposable
{
    Task<bool> Open(CancellationToken cancellationToken);
    Task Send(string message);
    WebSocketState State { get; }
    Task Closed { get; }
    IObservable<string> Messages { get; }
}

public class WebSocketWrapper(ClientWebSocket webSocket, string uri) : IWebSocket
{
    private readonly TaskCompletionSource<int> _closed = new();
    private readonly Subject<string> _messages = new();

    public int InitialBufferBytes { get; set; } = 1024;

    public async Task<bool> Open(CancellationToken cancellationToken)
    {
        await webSocket.ConnectAsync(new Uri(uri), cancellationToken).ConfigureAwait(false);
        
        if (webSocket.State == WebSocketState.Open)
        {
            _ = Task.Run(ReceiveLoop, CancellationToken.None);
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[InitialBufferBytes];
        var bufferSegment = new ArraySegment<byte>(buffer);

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(bufferSegment, CancellationToken.None).ConfigureAwait(false);

            switch (result.MessageType)
            {
                case WebSocketMessageType.Text when result.EndOfMessage:
                    var message = Encoding.UTF8.GetString(buffer, 0, bufferSegment.Offset + result.Count);
                    _messages.OnNext(message);
                    bufferSegment = new ArraySegment<byte>(buffer);
                    break;
                
                case WebSocketMessageType.Text:
                    if (result.Count == bufferSegment.Count)
                        Array.Resize(ref buffer, buffer.Length * 2);

                    bufferSegment = new ArraySegment<byte>(buffer, bufferSegment.Offset + result.Count, buffer.Length - bufferSegment.Offset - result.Count);
                    break;
                
                case WebSocketMessageType.Close:
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close requested by server", CancellationToken.None).ConfigureAwait(false);
                    break;
                
                case WebSocketMessageType.Binary:
                default:
                    // Ignored
                    break;
            }
        }
        
        _closed.SetResult(0);
    }

    public Task Send(string message) => webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);

    public WebSocketState State => webSocket.State;

    public Task Closed => _closed.Task;

    public IObservable<string> Messages => _messages.AsObservable();

    public void Dispose()
    {
        webSocket.Dispose();
        _messages.Dispose();
    }
}