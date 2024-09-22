using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using EasyAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NUnit.Framework;
using SlackNet.WebApi;

namespace SlackNet.Tests;

public class SlackRtmClientTests
{
    private SlackRtmClient _sut;
    private ISlackApiClient _slackApiClient;
    private SlackJsonSettings _jsonSettings;
    private TestScheduler _scheduler;
    private TestWebSocketFactory _webSocketFactory;

    [SetUp]
    public void SetUp()
    {
        _slackApiClient = Substitute.For<ISlackApiClient>();
        _webSocketFactory = new TestWebSocketFactory();
        _jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
        _scheduler = new TestScheduler();
        _sut = new SlackRtmClient(_slackApiClient, _webSocketFactory, _jsonSettings, _scheduler, Default.Logger);
    }

    [TestCase(true, true)]
    [TestCase(false, false)]
    public void Connect_OpensRtmWebSocket(bool manualPresenceSubscription, bool batchPresenceAware)
    {
        var cancellationToken = new CancellationToken(false);
        var rtmUrl = "rtm url";
        var socket = _webSocketFactory.Created.FirstAsync().ToTask();
        _slackApiClient.Rtm.Connect(manualPresenceSubscription, batchPresenceAware, cancellationToken)
            .Returns(new ConnectResponse { Url = rtmUrl });

        var result = _sut.Connect(batchPresenceAware, manualPresenceSubscription, cancellationToken);

        socket.ShouldComplete();
        socket.Result.Uri.ShouldBe(rtmUrl);
        
        socket.Result.Connection.SetResult(true);
        _scheduler.Start();
        
        result.ShouldComplete();
    }

    [Test]
    public void Connect_WebSocketClosed_Reconnects()
    {
        var firstSocket = _webSocketFactory.Created.ElementAt(0).ToTask();
        var secondSocket = _webSocketFactory.Created.ElementAt(1).ToTask();
        _slackApiClient.Rtm.Connect().ReturnsForAnyArgs(new ConnectResponse());
        
        var connected = _sut.Connect();

        firstSocket.ShouldComplete();
        firstSocket.Result.Connection.SetResult(true);
        _scheduler.Start();

        connected.ShouldComplete();

        firstSocket.Result.Dispose();
        _scheduler.Start();

        secondSocket.ShouldComplete();
    }
}