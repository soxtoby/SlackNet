#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using EasyAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SlackNet.SocketMode;
using SlackNet.WebApi;
using WebSocket4Net;

namespace SlackNet.Tests;

public class CoreSocketModeClientTests
{
    private CoreSocketModeClient _sut = null!;
    private TestWebSocketFactory _sockets = null!;
    private SlackJsonSettings _jsonSettings = null!;
    private TestLogger _logger = null!;
    private List<SocketMessage> _messages = null!;

    [SetUp]
    public void Setup()
    {
        var slack = Substitute.For<ISlackApiClient>();
        _sockets = new TestWebSocketFactory();
        _jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
        _logger = new TestLogger();
        _sut = new CoreSocketModeClient(
            slack,
            _sockets,
            _jsonSettings,
            Scheduler.Default,
            _logger);

        _messages = new List<SocketMessage>();
        _sut.Messages.Subscribe(_messages.Add);
            
        slack.AppsConnectionsApi.Open(Arg.Any<CancellationToken>()).Returns(new ConnectionOpenResponse { Ok = true, Url = "some url" });
    }

    [Test]
    public void Connected_ValidMessageReceived_OutputsMessage()
    {
        Connect();

        _sockets.Created.Last().Receive(JsonConvert.SerializeObject(new Hello { Type = "hello" }, _jsonSettings.SerializerSettings));

        _messages.ShouldBeASingular<Hello>();
    }

    [Test]
    public void Connected_NonJsonMessageReceived_IgnoresMessage()
    {
        Connect();
            
        _sockets.Created.Last().Receive("not json");

        _messages.ShouldBeEmpty();
        _logger.Events.ShouldContain("not json", (e, m) =>
            e is { Category: LogCategory.Error, Exception: not null }
            && e.Properties["Message"] == m);
    }

    private void Connect()
    {
        _sut.Connect(new SocketModeConnectionOptions { NumberOfConnections = 1 }).ShouldComplete();
    }
}

class TestWebSocketFactory : IWebSocketFactory
{
    public List<TestWebSocket> Created { get; } = new();

    public IWebSocket Create(string uri)
    {
        var socket = new TestWebSocket(uri);
        Created.Add(socket);
        return socket;
    }
}

class TestWebSocket : IWebSocket
{
    private readonly Subject<Unit> _opened = new();
    private readonly Subject<Unit> _closed = new();
    private readonly Subject<string> _messages = new();

    public TestWebSocket(string uri) => Uri = uri;

    public string Uri { get; }

    public List<string> Sent { get; } = new();

    public void Open()
    {
        State = WebSocketState.Open;
        _opened.OnNext(Unit.Default);
    }

    public void Send(string message) => Sent.Add(message);

    public void Receive(string message) => _messages.OnNext(message);

    public WebSocketState State { get; private set; }
    public IObservable<Unit> Opened => _opened;
    public IObservable<Unit> Closed => _closed;
    public IObservable<Exception> Errors => Observable.Never<Exception>();
    public IObservable<string> Messages => _messages;

    public void Dispose()
    {
        _closed.OnNext(Unit.Default);
    }
}

class TestLogger : ILogger
{
    public List<ILogEvent> Events { get; } = new();
        
    public void Log(ILogEvent logEvent)
    {
        Events.Add(logEvent);
        TestContext.WriteLine($"[{logEvent.Category}] {logEvent.FullMessage()}");
    }
}