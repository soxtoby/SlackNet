#nullable enable
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using EasyAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SlackNet.SocketMode;
using SlackNet.WebApi;

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

        _messages = [];
        _sut.Messages.Subscribe(_messages.Add);
            
        slack.AppsConnectionsApi.Open(Arg.Any<CancellationToken>()).Returns(new ConnectionOpenResponse { Ok = true, Url = "some url" });
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