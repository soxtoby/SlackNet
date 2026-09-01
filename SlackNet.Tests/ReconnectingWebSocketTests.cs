#nullable enable
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using EasyAssertions;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using SlackNet.WebApi;

namespace SlackNet.Tests;

public class ReconnectingWebSocketTests
{
    private ReconnectingWebSocket _sut;
    private TestWebSocketFactory _webSocketFactory;
    private TestScheduler _scheduler;
    private TestLogger _logger;

    [SetUp]
    public void SetUp()
    {
        _scheduler = new TestScheduler();
        _webSocketFactory = new TestWebSocketFactory();
        _logger = new TestLogger();
        _sut = new ReconnectingWebSocket(_webSocketFactory, _scheduler, _logger, 0);
    }

    [Test]
    public async Task Connect_Succeeds_CompletesAsConnected()
    {
        var socket = _webSocketFactory.Created.FirstAsync().ToTask();

        var result = _sut.Connect(() => Task.FromResult("test url"));

        SocketConnected(socket, true);

        result.ShouldComplete();
        _sut.State.ShouldBe(WebSocketState.Open);
    }

    [Test]
    public void Connect_Cancelled_FailsWithCancellation()
    {
        using var cts = new CancellationTokenSource();
        var createdSocket = _webSocketFactory.Created.FirstAsync().ToTask();

        var result = _sut.Connect(() => Task.FromResult("test url"), cts.Token);
        createdSocket.ShouldComplete();

        cts.Cancel();
        result.ShouldFailWith<TaskCanceledException>();
    }

    [Test]
    public void Connect_Fails_Retries()
    {
        var socket0 = _webSocketFactory.Created.ElementAt(0).ToTask();
        var socket1 = _webSocketFactory.Created.ElementAt(1).ToTask();
        var socket2 = _webSocketFactory.Created.ElementAt(2).ToTask();

        var result = _sut.Connect(() => Task.FromResult("test url"));

        result.IsCompleted.ShouldBe(false);
        SocketConnected(socket0, false);

        result.IsCompleted.ShouldBe(false);
        SocketConnectionFailed(socket1);

        result.IsCompleted.ShouldBe(false);
        SocketConnected(socket2, true);
        
        result.ShouldComplete();
        _sut.State.ShouldBe(WebSocketState.Open);
    }

    [Test]
    public void Closed_Reconnects()
    {
        var socket0 = _webSocketFactory.Created.ElementAt(0).ToTask();
        var socket1 = _webSocketFactory.Created.ElementAt(1).ToTask();
        var connected = _sut.Connect(() => Task.FromResult("test url"));
        SocketConnected(socket0, true);
        connected.ShouldComplete();

        socket0.Result.Dispose();
        _scheduler.Start();

        SocketConnected(socket1, true);
        _sut.State.ShouldBe(WebSocketState.Open);
    }

    [Test]
    public void Closed_ReconnectFails_LogsError()
    {
        var socket = _webSocketFactory.Created.FirstAsync().ToTask();
        var urlRequests = 0;
        var connected = _sut.Connect(() =>
            ++urlRequests == 1
                ? Task.FromResult("test url")
                : Task.FromException<string>(new SlackException(new ErrorResponse { Error = "fatal_test_error" })));
        SocketConnected(socket, true);
        connected.ShouldComplete();

        socket.Result.Dispose();
        _scheduler.Start();

        _logger.Events.Any(e => e is { Category: LogCategory.Error, Exception: SlackException }).ShouldBe(true);
    }

    [Test]
    public void CancelledAfterConnecting_DoesNotReconnect()
    {
        using var cancellation = new CancellationTokenSource();
        var sockets = _scheduler.CreateObserver<TestWebSocket>();
        _webSocketFactory.Created.Subscribe(sockets);
        var socket = _webSocketFactory.Created.FirstAsync().ToTask();
        var connected = _sut.Connect(() => Task.FromResult("test url"), cancellation.Token);
        SocketConnected(socket, true);
        connected.ShouldComplete();

        cancellation.Cancel();
        socket.Result.Dispose();
        _scheduler.Start();

        sockets.Messages.ShouldBeLength(1);
    }

    [Test]
    public void Disposed_Disconnects()
    {
        var sockets = _scheduler.CreateObserver<TestWebSocket>();
        _webSocketFactory.Created.Subscribe(sockets);
        var socket = _webSocketFactory.Created.FirstAsync().ToTask();
        var connected = _sut.Connect(() => Task.FromResult("test url"));
        SocketConnected(socket, true);
        connected.ShouldComplete();
        
        _sut.Dispose();

        _sut.State.ShouldBe(WebSocketState.Closed);
        sockets.Messages.ShouldBeLength(1);
    }

    private void SocketConnected(Task<TestWebSocket> socket, bool connected)
    {
        socket.ShouldComplete();
        socket.Result.Connection.SetResult(connected);
        _scheduler.Start();
    }

    private void SocketConnectionFailed(Task<TestWebSocket> socket)
    {
        socket.ShouldComplete();
        socket.Result.Connection.SetException(new Exception("unexpected error"));
        _scheduler.Start();
    }
}
