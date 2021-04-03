using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using EasyAssertions;
using Microsoft.Reactive.Testing;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SlackNet.WebApi;
using WebSocket4Net;

namespace SlackNet.Tests
{
    public class SlackRtmClientTests
    {
        private SlackRtmClient _sut;
        private ISlackApiClient _slackApiClient;
        private SlackJsonSettings _jsonSettings;
        private TestScheduler _testScheduler;
        private IWebSocketFactory _webSocketFactory;
        private Subject<Unit> _opened;
        private Subject<Unit> _closed;
        private Subject<Exception> _errors;
        private Subject<string> _messages;
        private IWebSocket _webSocket;

        [SetUp]
        public void SetUp()
        {
            _slackApiClient = Substitute.For<ISlackApiClient>();
            _webSocketFactory = Substitute.For<IWebSocketFactory>();
            _jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
            _testScheduler = new TestScheduler();
            _sut = new SlackRtmClient(_slackApiClient, _webSocketFactory, _jsonSettings, _testScheduler, Default.Logger);

            _opened = new Subject<Unit>();
            _closed = new Subject<Unit>();
            _errors = new Subject<Exception>();
            _messages = new Subject<string>();
            _webSocket = Substitute.For<IWebSocket>();
            _webSocket.Opened.Returns(_opened);
            _webSocket.Closed.Returns(_closed);
            _webSocket.Errors.Returns(_errors);
            _webSocket.Messages.Returns(_messages);
            _webSocket.State.Returns(WebSocketState.None);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Connect_OpensRtmWebSocket(bool manualPresenceSubscription, bool batchPresenceAware)
        {
            var cancellationToken = new CancellationToken(false);
            var rtmUrl = "rtm url";
            _slackApiClient.Rtm.Connect(manualPresenceSubscription, batchPresenceAware, cancellationToken)
                .Returns(new ConnectResponse { Url = rtmUrl });
            _webSocketFactory.Create(rtmUrl).Returns(_webSocket);

            var result = _sut.Connect(batchPresenceAware, manualPresenceSubscription, cancellationToken);

            result.IsCompleted.ShouldBe(false);
            _webSocket.Received().Open();

            _opened.OnNext(Unit.Default);

            result.ShouldComplete();
        }

        [Test]
        public void Connect_ErrorBeforeOpened_Fails()
        {
            SetUpRtm();

            var result = _sut.Connect();

            var exception = new Exception();
            _errors.OnNext(exception);

            result.ShouldFail()
                .And.ShouldBe(exception);
        }

        [Test]
        public void Connect_WebSocketClosed_Reconnects()
        {
            ConnectSuccessfully();

            _closed.OnNext(Unit.Default);

            _webSocket.Received(2).Open();
        }

        [Test]
        public void Connect_WebSocketClosed_ReconnectFails_RetryAfterSomeTime()
        {
            ConnectSuccessfully();

            _closed.OnNext(Unit.Default);
            _webSocket.Received(2).Open(); // Should reconnect immediately

            _errors.OnNext(new Exception());
            _testScheduler.AdvanceBy(1); // Required for Catch

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks - 1);
            _webSocket.Received(2).Open(); // Still hasn't retried

            _testScheduler.AdvanceBy(1); // 1 second since catch
            _webSocket.Received(3).Open();
        }

        private void ConnectSuccessfully()
        {
            SetUpRtm();
            var _ = _sut.Connect();
            _opened.OnNext(Unit.Default);
            _webSocket.Received(1).Open();
        }

        private void SetUpRtm()
        {
            _slackApiClient.Rtm.Connect().ReturnsForAnyArgs(new ConnectResponse());
            _webSocketFactory.Create(Arg.Any<string>()).Returns(_webSocket);
        }
    }
}