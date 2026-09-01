#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using EasyAssertions;
using Microsoft.Reactive.Testing;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SlackNet.SocketMode;
using SlackNet.WebApi;

namespace SlackNet.Tests;

public class CoreSocketModeClientTests
{
    private CoreSocketModeClient _sut = null!;
    private ISlackApiClient _slack = null!;
    private TestWebSocketFactory _sockets = null!;
    private SlackJsonSettings _jsonSettings = null!;
    private TestLogger _logger = null!;
    private List<SocketMessage> _messages = null!;

    [SetUp]
    public void Setup()
    {
        _slack = Substitute.For<ISlackApiClient>();
        _sockets = new TestWebSocketFactory();
        _jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
        _logger = new TestLogger();
        _sut = new CoreSocketModeClient(
            _slack,
            _sockets,
            _jsonSettings,
            Scheduler.Default,
            _logger);

        _messages = [];
        _sut.Messages.Subscribe(_messages.Add);
            
        _slack.AppsConnectionsApi.Open(Arg.Any<CancellationToken>()).Returns(new ConnectionOpenResponse { Ok = true, Url = "some url" });
    }

    [Test]
    public void Connected_ValidMessageReceived_OutputsMessage()
    {
        var socket = Connect();

        socket.Receive(JsonConvert.SerializeObject(new Hello { Type = "hello" }, _jsonSettings.SerializerSettings));

        _messages.ShouldBeASingular<Hello>();
    }

    [Test]
    public void Connected_NonJsonMessageReceived_IgnoresMessage()
    {
        var socket = Connect();
            
        socket.Receive("not json");

        _messages.ShouldBeEmpty();
        _logger.Events.ShouldContain("not json", (e, m) =>
            e is { Category: LogCategory.Error, Exception: not null }
            && e.Properties["Message"] == m);
    }

    [Test]
    public void DisconnectAsync_Connected_StopsConnection()
    {
        var socket = Connect();

        var disconnected = _sut.DisconnectAsync();

        disconnected.ShouldComplete();
        _sut.Connected.ShouldBe(false);
        socket.State.ShouldBe(WebSocketState.Closed);
    }

    [Test]
    public void DisconnectAsync_WhileDisconnecting_AwaitsActiveTeardown()
    {
        var socket = Connect();
        var cancellationStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var continueCancellation = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var cancellationRegistration = socket.OpenCancellationToken.Register(() =>
            {
                cancellationStarted.TrySetResult();
                continueCancellation.Task.GetAwaiter().GetResult();
            });

        var firstDisconnect = _sut.DisconnectAsync();
        cancellationStarted.Task.ShouldComplete();

        var secondDisconnect = _sut.DisconnectAsync();
        try
        {
            firstDisconnect.IsCompleted.ShouldBe(false);
            secondDisconnect.IsCompleted.ShouldBe(false);
            ReferenceEquals(firstDisconnect, secondDisconnect).ShouldBe(true);
        }
        finally
        {
            continueCancellation.TrySetResult();
        }

        Task.WhenAll(firstDisconnect, secondDisconnect).ShouldComplete();
    }

    [Test]
    public void Connect_WhileConnecting_ThrowsWithoutReplacingConnection()
    {
        var socket = _sockets.Created.FirstAsync().ToTask();
        var firstConnect = _sut.Connect(new SocketModeConnectionOptions { NumberOfConnections = 1 });
        socket.ShouldComplete();

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.Connect(new SocketModeConnectionOptions { NumberOfConnections = 1 }));
        _sockets.Created.Take(2).ToTask().IsCompleted.ShouldBe(false);

        socket.Result.Connection.SetResult(true);
        firstConnect.ShouldComplete();
    }

    [Test]
    public void SocketModeDisabled_DoesNotBlockMessageDelivery()
    {
        var socket = Connect();
        var cancellationStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var continueCancellation = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var cancellationRegistration = socket.OpenCancellationToken.Register(() =>
            {
                cancellationStarted.TrySetResult();
                continueCancellation.Task.GetAwaiter().GetResult();
            });

        var receive = Task.Run(() => socket.Receive(JsonConvert.SerializeObject(
            new Disconnect { Type = "disconnect", Reason = DisconnectReason.SocketModeDisabled },
            _jsonSettings.SerializerSettings)));
        cancellationStarted.Task.ShouldComplete();

        try
        {
            receive.IsCompleted.ShouldBe(true);
        }
        finally
        {
            continueCancellation.TrySetResult();
        }

        receive.ShouldComplete();
        _sut.DisconnectAsync().ShouldComplete();
    }

    [Test]
    public void AdditionalConnection_Fails_LogsError()
    {
        var scheduler = new TestScheduler();
        _sut.Dispose();
        _sut = new CoreSocketModeClient(_slack, _sockets, _jsonSettings, scheduler, _logger);
        _slack.AppsConnectionsApi.Open(Arg.Any<CancellationToken>()).Returns(
            Task.FromResult(new ConnectionOpenResponse { Ok = true, Url = "some url" }),
            Task.FromException<ConnectionOpenResponse>(new SlackException(new ErrorResponse { Error = "fatal_test_error" })));
        var socket = _sockets.Created.FirstAsync().ToTask();

        var connected = _sut.Connect(new SocketModeConnectionOptions
            {
                NumberOfConnections = 2,
                ConnectionDelay = TimeSpan.FromSeconds(1)
            });
        socket.ShouldComplete();
        socket.Result.Connection.SetResult(true);
        scheduler.Start();
        connected.ShouldComplete();
        _sut.DisconnectAsync().ShouldComplete();

        _logger.Events.Any(e => e is { Category: LogCategory.Error, Exception: SlackException }).ShouldBe(true);
    }

    private TestWebSocket Connect()
    {
        var socket = _sockets.Created.FirstAsync().ToTask();

        var connected = _sut.Connect(new SocketModeConnectionOptions { NumberOfConnections = 1 });

        socket.ShouldComplete();
        socket.Result.Connection.SetResult(true);
        
        connected.ShouldComplete();

        return socket.Result;
    }
}
