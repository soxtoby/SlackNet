using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using EasyAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using SlackNet.Bot;
using SlackNet.Events;
using SlackNet.Rtm;
using SlackNet.WebApi;
using BotMessage = SlackNet.Bot.BotMessage;

// ReSharper disable ConsiderUsingConfigureAwait
namespace SlackNet.Tests
{
    public class SlackBotTests
    {
        private SlackBot _sut;
        private ISlackRtmClient _rtm;
        private ISlackApiClient _api;
        private TestScheduler _scheduler;
        private Subject<MessageEvent> _incomingMessages;

        [SetUp]
        public void SetUp()
        {
            _rtm = Substitute.For<ISlackRtmClient>();
            _incomingMessages = new Subject<MessageEvent>();
            _rtm.Messages.Returns(_incomingMessages);

            _api = Substitute.For<ISlackApiClient>();
            _scheduler = new TestScheduler();
            _scheduler.AdvanceTo(TimeSpan.FromMinutes(1).Ticks); // So first message isn't queued
            _sut = new SlackBot(_rtm, _api, _scheduler);
        }

        [Test]
        public async Task Connect_SetsIdAndName()
        {
            _rtm.Connect().Returns(new ConnectResponse { Self = new Self { Id = "foo", Name = "bar" } });

            await _sut.Connect().ConfigureAwait(false);

            _sut.Id.ShouldBe("foo");
            _sut.Name.ShouldBe("bar");
        }

        [Test]
        public async Task AddIncomingMiddleware_AppliedToIncomingMessages()
        {
            _sut.AddIncomingMiddleware(ms => ms.Select(m =>
                {
                    m.Text += "++";
                    return m;
                }));
            await Connect().ConfigureAwait(false);
            var observer = ObserveIncomingMessages();

            _incomingMessages.OnNext(new MessageEvent { Text = "foo" });

            observer.Messages.Select(m => m.Value.Value.Text)
                .ShouldMatch(new[] { "foo++" });
        }

        [Test]
        public async Task AddOutgoingMiddleware_AppliedToOutgoingMessages()
        {
            _sut.AddOutgoingMiddleware(ms => ms.Select(m =>
                {
                    m.Text += "++";
                    return m;
                }));
            await Connect().ConfigureAwait(false);

            _sut.OnNext(new BotMessage { Text = "foo" });

            await _api.Chat.Received().PostMessage(Arg.Is<Message>(m => m.Text == "foo++"), Arg.Any<CancellationToken?>()).ConfigureAwait(false);
        }

        [Test]
        public async Task AddHandler_HandlesMessages()
        {
            var handler = Substitute.For<IMessageHandler>();
            await Connect().ConfigureAwait(false);

            _sut.AddHandler(handler);
            _incomingMessages.OnNext(new MessageEvent { Text = "foo" });

            await handler.Received().HandleMessage(Arg.Is<IMessage>(m => m.Text == "foo")).ConfigureAwait(false);
        }

        [Test]
        public async Task OnMessageEvent_HandlesMessages()
        {
            var eventHandler = Substitute.For<EventHandler<IMessage>>();
            await Connect().ConfigureAwait(false);

            _sut.OnMessage += eventHandler;
            _incomingMessages.OnNext(new MessageEvent { Text = "foo" });

            eventHandler.Received()(_sut, Arg.Is<IMessage>(m => m.Text == "foo"));
        }

        [Test]
        public async Task Messages_ReceivesMessages()
        {
            var observer = _scheduler.CreateObserver<IMessage>();
            await Connect().ConfigureAwait(false);

            _sut.Messages.Subscribe(observer);
            _incomingMessages.OnNext(new MessageEvent { Text = "foo" });

            observer.Messages.Select(m => m.Value.Value.Text)
                .ShouldMatch(new[] { "foo" });
        }

        [Test]
        public async Task IncomingMessage_MapsProperties()
        {
            await Connect().ConfigureAwait(false);
            var incoming = new MessageEvent
            {
                Ts = "1",
                ThreadTs = "2",
                Text = "foo",
                Channel = "C123",
                User = "U456",
                Attachments = { new Attachment { Text = "bar" } }
            };
            var channel = new Channel { Name = "channel" };
            _api.Channels.Info(incoming.Channel).Returns(channel);
            var user = new User { Name = "user" };
            _api.Users.Info(incoming.User).Returns(user);
            var observer = _scheduler.CreateObserver<IMessage>();
            _sut.Messages.Subscribe(observer);

            _incomingMessages.OnNext(incoming);

            var result = observer.Messages[0].Value.Value;
            result.Hub.ShouldBe(channel);
            result.User.ShouldBe(user);
            result.Text.ShouldBe(incoming.Text);
            result.Ts.ShouldBe(incoming.Ts);
            result.ThreadTs.ShouldBe(incoming.ThreadTs);
            result.Attachments.ShouldMatch(incoming.Attachments);
        }

        [Test]
        public async Task IncomingMessage_CanReply()
        {
            await Connect().ConfigureAwait(false);
            var observer = _scheduler.CreateObserver<IMessage>();
            _sut.Messages.Subscribe(observer);

            _incomingMessages.OnNext(new MessageEvent());
            await observer.Messages[0].Value.Value.ReplyWith("foo").ConfigureAwait(false);

            await _api.Chat.Received().PostMessage(Arg.Is<Message>(m => m.Text == "foo"), Arg.Any<CancellationToken?>()).ConfigureAwait(false);
        }

        [Test]
        public async Task IncomingMessages_OnlyPlainMessagesHandled()
        {
            await Connect().ConfigureAwait(false);
            var observer = _scheduler.CreateObserver<IMessage>();
            _sut.Messages.Subscribe(observer);

            _incomingMessages.OnNext(new MessageChanged { Text = "foo" });
            _incomingMessages.OnNext(new MessageEvent { Text = "bar" });

            observer.Messages.Select(m => m.Value.Value.Text)
                .ShouldMatch(new[] { "bar" });
        }

        [Test]
        public void GetHubById_NullId_ReturnsNull()
        {
            _sut.GetHubById(null)
                .ShouldComplete()
                .And.ShouldBeNull();
        }

        [Test]
        public void GetHubById_UnknownIdType_ReturnsNull()
        {
            _sut.GetHubById("foo")
                .ShouldComplete()
                .And.ShouldBeNull();
        }

        [Test]
        public void GetHubById_ChannelId_ReturnsChannelInfoFromApi_AndIsCached()
        {
            var channelId = "C123";
            var expectedChannel = new Channel();
            _api.Channels.Info(channelId).Returns(expectedChannel);

            _sut.GetHubById(channelId)
                .ShouldComplete()
                .And.ShouldBe(expectedChannel);
            _sut.GetHubById(channelId)
                .ShouldComplete()
                .And.ShouldBe(expectedChannel);
            _api.Channels.Received(1).Info(channelId);
        }

        [Test]
        public void GetHubById_GroupId_ReturnsGroupInfoFromApi_AndIsCached()
        {
            var groupId = "G123";
            var expectedGroup = new Channel();
            _api.Groups.Info(groupId).Returns(expectedGroup);

            _sut.GetHubById(groupId)
                .ShouldComplete()
                .And.ShouldBe(expectedGroup);
            _sut.GetHubById(groupId)
                .ShouldComplete()
                .And.ShouldBe(expectedGroup);
            _api.Groups.Received(1).Info(groupId);
        }

        [Test]
        public void GetHubById_ImId_ReturnsImInfoFromApi_AndIsCached()
        {
            var imId = "D123";
            var expectedIm = new Im();
            _api.Conversations.OpenAndReturnInfo(imId).Returns(new ImResponse { Channel = expectedIm });

            _sut.GetHubById(imId)
                .ShouldComplete()
                .And.ShouldBe(expectedIm);
            _sut.GetHubById(imId)
                .ShouldComplete()
                .And.ShouldBe(expectedIm);
            _api.Conversations.Received(1).OpenAndReturnInfo(imId);
        }

        [Test]
        public void GetHubByName_ChannelName_FindsChannelWithMatchingName()
        {
            var expectedChannel = new Channel { Id = "C1", Name = "foo" };
            var otherChannel = new Channel { Id = "C2", Name = "bar" };
            _api.Channels.List().Returns(new[] { otherChannel, expectedChannel });

            _sut.GetHubByName("#foo")
                .ShouldComplete()
                .And.ShouldBe(expectedChannel);
        }

        [Test]
        public void GetHubByName_UserName_FindsImWithMatchingUser()
        {
            var matchingUser = new User { Id = "U1", Name = "foo" };
            var otherUser = new User { Id = "U2", Name = "bar" };
            _api.Users.List().Returns(new UserListResponse { Members = { otherUser, matchingUser } });
            var expectedIm = new Im { Id = "D123" };
            _api.Im.Open(matchingUser.Id, true).Returns(new ImResponse { Channel = expectedIm });

            _sut.GetHubByName("@foo")
                .ShouldComplete()
                .And.ShouldBe(expectedIm);
        }

        [Test]
        public void GetHubByName_GroupName_FindsGroupWithMatchingName()
        {
            var expectedGroup = new Channel { Id = "G1", Name = "foo" };
            var otherGroup = new Channel { Id = "G2", Name = "bar" };
            _api.Groups.List().Returns(new[] { otherGroup, expectedGroup });

            _sut.GetHubByName("foo")
                .ShouldComplete()
                .And.ShouldBe(expectedGroup);
        }

        [Test]
        public void GetChannelByName_FindsChannelWithMatchingName_AndCaches()
        {
            var expectedChannel = new Channel { Id = "C1", Name = "foo" };
            var otherChannel = new Channel { Id = "C2", Name = "bar" };
            _api.Channels.List().Returns(new[] { otherChannel, expectedChannel });

            _sut.GetChannelByName("#foo")
                .ShouldComplete()
                .And.ShouldBe(expectedChannel);
            _sut.GetChannelByName("foo")
                .ShouldComplete()
                .And.ShouldBe(expectedChannel);
            _api.Channels.Received(1).List();
        }

        [Test]
        public void GetImByName_FindsImWithMatchingUserName_AndCaches()
        {
            var matchingUser = new User { Id = "U1", Name = "foo" };
            var otherUser = new User { Id = "U2", Name = "bar" };
            _api.Users.List().Returns(new UserListResponse { Members = { otherUser, matchingUser } });
            var expectedIm = new Im { Id = "D123", User = matchingUser.Id };
            _api.Im.Open(matchingUser.Id, true).Returns(new ImResponse { Channel = expectedIm });

            _sut.GetImByName("@foo")
                .ShouldComplete()
                .And.ShouldBe(expectedIm);
            _sut.GetImByName("foo")
                .ShouldComplete()
                .And.ShouldBe(expectedIm);
            _api.Users.Received(1).List();
            _api.Im.ReceivedWithAnyArgs(1).Open(Arg.Any<string>());
        }

        [Test]
        public void GetGroupByName_FindsGroupWithMatchingName_AndCaches()
        {
            var expectedGroup = new Channel { Id = "G1", Name = "foo", IsGroup = true };
            var otherGroup = new Channel { Id = "G2", Name = "bar", IsGroup = true };
            _api.Groups.List().Returns(new[] { otherGroup, expectedGroup });

            _sut.GetGroupByName("foo")
                .ShouldComplete()
                .And.ShouldBe(expectedGroup);
            _sut.GetGroupByName("foo")
                .ShouldComplete()
                .And.ShouldBe(expectedGroup);
            _api.Groups.Received(1).List();
        }

        [Test]
        public void GetImByUserId_OpensImWithUser_AndCaches()
        {
            var expectedIm = new Im { Id = "D123", User = "U123" };
            _api.Im.Open(expectedIm.User, true).Returns(new ImResponse { Channel = expectedIm });

            _sut.GetImByUserId(expectedIm.User)
                .ShouldComplete()
                .And.ShouldBe(expectedIm);
            _sut.GetImByUserId(expectedIm.User)
                .ShouldComplete()
                .And.ShouldBe(expectedIm);
            _api.Im.Received(1).Open(expectedIm.User, true);
        }

        [Test]
        public void GetChannels_FetchesChannelList_AndCaches()
        {
            var channel1 = new Channel { Id = "C1" };
            var channel2 = new Channel { Id = "C2" };
            _api.Channels.List().Returns(new[] { channel1, channel2 });

            _sut.GetChannels()
                .ShouldComplete()
                .And.ShouldMatch(new[] { channel1, channel2 });
            _sut.GetChannels()
                .ShouldComplete()
                .And.ShouldMatch(new[] { channel1, channel2 });
            _api.Channels.Received(1).List();
        }

        [Test]
        public void GetGroups_FetchesGroupList_AndCaches()
        {
            var group1 = new Channel { Id = "G1" };
            var group2 = new Channel { Id = "G2" };
            _api.Groups.List().Returns(new[] { group1, group2 });

            _sut.GetGroups()
                .ShouldComplete()
                .And.ShouldMatch(new[] { group1, group2 });
            _sut.GetGroups()
                .ShouldComplete()
                .And.ShouldMatch(new[] { group1, group2 });
            _api.Groups.Received(1).List();
        }

        [Test]
        public void GetMpIms_FetchesMpImList_AndCaches()
        {
            var mpim1 = new Channel { Id = "G1" };
            var mpim2 = new Channel { Id = "G2" };
            _api.Mpim.List().Returns(new[] { mpim1, mpim2 });

            _sut.GetMpIms()
                .ShouldComplete()
                .And.ShouldMatch(new[] { mpim1, mpim2 });
            _sut.GetMpIms()
                .ShouldComplete()
                .And.ShouldMatch(new[] { mpim1, mpim2 });
            _api.Mpim.Received(1).List();
        }

        [Test]
        public void GetIms_FetchesOpenIms_AndCaches()
        {
            var im1 = new Im { Id = "D1" };
            var im2 = new Im { Id = "D2" };
            _api.Im.List().Returns(new[] { im1, im2 });

            _sut.GetIms()
                .ShouldComplete()
                .And.ShouldMatch(new[] { im1, im2 });
            _sut.GetIms()
                .ShouldComplete()
                .And.ShouldMatch(new[] { im1, im2 });
            _api.Im.Received(1).List();
        }

        [Test]
        public void GetUserById_NullId_ReturnsNull()
        {
            _sut.GetUserById(null)
                .ShouldComplete()
                .And.ShouldBeNull();
        }

        [Test]
        public void GetUserById_ReturnsUser_AndCaches()
        {
            var userId = "U123";
            var expectedUser = new User { Id = userId };
            _api.Users.Info(userId).Returns(expectedUser);

            _sut.GetUserById(userId)
                .ShouldComplete()
                .And.ShouldBe(expectedUser);
            _sut.GetUserById(userId)
                .ShouldComplete()
                .And.ShouldBe(expectedUser);
            _api.Users.Received(1).Info(userId);
        }

        [Test]
        public void GetUserByName_FindsUserWithMatchingName_AndCaches()
        {
            var expectedUser = new User { Id = "U1", Name = "foo" };
            var otherUser = new User { Id = "U2", Name = "bar" };
            _api.Users.List().Returns(new UserListResponse { Members = { otherUser, expectedUser } });

            _sut.GetUserByName("@foo")
                .ShouldComplete()
                .And.ShouldBe(expectedUser);
            _sut.GetUserByName("foo")
                .ShouldComplete()
                .And.ShouldBe(expectedUser);
            _api.Users.Received(1).List();
        }

        [Test]
        public void GetUsers_ReturnsUserList_AndCaches()
        {
            var user1 = new User { Id = "U1" };
            var user2 = new User { Id = "U2" };
            _api.Users.List().Returns(new UserListResponse { Members = { user1, user2 } });

            _sut.GetUsers()
                .ShouldComplete()
                .And.ShouldMatch(new[] { user1, user2 });
            _sut.GetUsers()
                .ShouldComplete()
                .And.ShouldMatch(new[] { user1, user2 });
            _api.Users.Received(1).List();
        }

        [Test]
        public void GetUsers_ReturnsFullUserList()
        {
            var user1 = new User { Id = "U1" };
            var user2 = new User { Id = "U2" };
            var user3 = new User { Id = "U3" };
            _api.Users.List().Returns(new UserListResponse { Members = { user1 }, ResponseMetadata = new ResponseMetadata { NextCursor = "foo" } });
            _api.Users.List("foo").Returns(new UserListResponse { Members = { user2 }, ResponseMetadata = new ResponseMetadata { NextCursor = "bar" } });
            _api.Users.List("bar").Returns(new UserListResponse { Members = { user3 } });

            _sut.GetUsers()
                .ShouldComplete()
                .And.ShouldMatch(new[] { user1, user2, user3 });
        }

        [Test]
        public async Task Send_ReplyInChannel()
        {
            var slackMessage = new SlackMessage(_sut)
            {
                Hub = new Channel { Id = "channel" },
                Ts = "123"
            };
            await Connect().ConfigureAwait(false);

            await _sut.Send(new BotMessage { ReplyTo = slackMessage, CreateThread = false }).ConfigureAwait(false);

            await _api.Chat.Received().PostMessage(Arg.Is<Message>(message => message.ThreadTs == null), Arg.Any<CancellationToken?>()).ConfigureAwait(false);
        }

        [Test]
        public async Task Send_ReplyInExistingThread()
        {
            var slackMessage = new SlackMessage(_sut)
            {
                Hub = new Channel { Id = "channel" },
                Ts = "123",
                ThreadTs = "456"
            };
            await Connect().ConfigureAwait(false);

            await _sut.Send(new BotMessage { ReplyTo = slackMessage, CreateThread = false }).ConfigureAwait(false);

            await _api.Chat.Received().PostMessage(Arg.Is<Message>(message => message.ThreadTs == slackMessage.ThreadTs), Arg.Any<CancellationToken?>()).ConfigureAwait(false);
        }

        [Test]
        public async Task Send_ReplyInNewThread()
        {
            var slackMessage = new SlackMessage(_sut)
            {
                Hub = new Channel { Id = "channel" },
                Ts = "123"
            };
            await Connect().ConfigureAwait(false);

            await _sut.Send(new BotMessage { ReplyTo = slackMessage, CreateThread = true }).ConfigureAwait(false);

            await _api.Chat.Received().PostMessage(Arg.Is<Message>(message => message.ThreadTs == slackMessage.Ts), Arg.Any<CancellationToken?>()).ConfigureAwait(false);
        }

        [Test]
        public async Task Send_ReplyInDifferentHub()
        {
            var slackMessage = new SlackMessage(_sut)
            {
                Hub = new Channel { Id = "channel" },
                Ts = "123",
                ThreadTs = "456"
            };
            await Connect().ConfigureAwait(false);

            await _sut.Send(new BotMessage { ReplyTo = slackMessage, Hub = new HubByRef(new Channel { Id = "other_channel" }) }).ConfigureAwait(false);

            await _api.Chat.Received().PostMessage(Arg.Is<Message>(message => message.ThreadTs == null && message.Channel == "other_channel"), Arg.Any<CancellationToken?>()).ConfigureAwait(false);
        }

        [Test]
        public async Task Send_MessagesLimitedTo1PerSecond()
        {
            await Connect().ConfigureAwait(false);

            var sent1 = _sut.Send(new BotMessage { Text = "foo" });
            var sent2 = _sut.Send(new BotMessage { Text = "bar" });

            await _api.Chat.Received().PostMessage(Arg.Is<Message>(message => message.Text == "foo"), Arg.Any<CancellationToken?>()).ConfigureAwait(false);
            await _api.Chat.DidNotReceive().PostMessage(Arg.Is<Message>(message => message.Text == "bar"), Arg.Any<CancellationToken?>()).ConfigureAwait(false);

            _scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);

            await _api.Chat.Received().PostMessage(Arg.Is<Message>(message => message.Text == "bar"), Arg.Any<CancellationToken?>()).ConfigureAwait(false);
        }

        [Test]
        public async Task Send_PostMessageFails_ExceptionPropagated()
        {
            var expectedException = new SlackException(new ErrorResponse());
            _api.Chat.PostMessage(Arg.Any<Message>(), Arg.Any<CancellationToken?>()).Throws(expectedException);
            await Connect().ConfigureAwait(false);

            _sut.Send(new BotMessage())
                .ShouldFail().And.ShouldBe(expectedException);
        }

        [Test]
        public void WhileTyping_SendsTypingEvery4SecondsUntilTaskCompletes()
        {
            var taskSource = new TaskCompletionSource<int>();
            var channelId = "C123";

            var result = _sut.WhileTyping(channelId, () => taskSource.Task);

            _scheduler.AdvanceBy(TimeSpan.FromSeconds(4).Ticks - 1);
            _rtm.DidNotReceive().SendTyping(channelId);

            _scheduler.AdvanceBy(1);
            _rtm.Received(1).SendTyping(channelId);

            _scheduler.AdvanceBy(TimeSpan.FromSeconds(8).Ticks);
            _rtm.Received(3).SendTyping(channelId);

            taskSource.SetResult(0);
            result.IsCompleted.ShouldBe(true);
            _scheduler.AdvanceBy(TimeSpan.FromSeconds(4).Ticks);
            _rtm.Received(3).SendTyping(channelId);
        }

        private async Task Connect()
        {
            _rtm.Connect().Returns(new ConnectResponse { Self = new Self { Id = "test_bot", Name = "TestBot" } });
            await _sut.Connect().ConfigureAwait(false);
        }

        private ITestableObserver<IMessage> ObserveIncomingMessages()
        {
            var observer = _scheduler.CreateObserver<IMessage>();
            _sut.Messages.Subscribe(observer);
            return observer;
        }
    }
}

