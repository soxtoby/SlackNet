#nullable enable
using System.Reactive.Subjects;

namespace SlackNet.Tests;

class TestWebSocketFactory : IWebSocketFactory
{
    public Subject<TestWebSocket> Created { get; } = new();

    public IWebSocket Create(string uri)
    {
        var socket = new TestWebSocket(uri);
        Created.OnNext(socket);
        return socket;
    }
}