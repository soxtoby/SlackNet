using System;
using System.Linq;
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
            var channel = new Conversation { Name = "channel" };
            _api.Conversations.Info(incoming.Channel).Returns(channel);
            var user = new User { Name = "user" };
            _api.Users.Info(incoming.User).Returns(user);
            var observer = _scheduler.CreateObserver<IMessage>();
            _sut.Messages.Subscribe(observer);

            _incomingMessages.OnNext(incoming);

            var result = observer.Messages[0].Value.Value;
            result.Conversation.ShouldBe(channel);
            result.Hub.ShouldBeA<Channel>().And.Name.ShouldBe(channel.Name);
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
        public void GetConversationById_NullId_ReturnsNull()
        {
            _sut.GetConversationById(null)
                .ShouldComplete()
                .And.ShouldBeNull();
        }

        [Test]
        public void GetConversationById_IdSpecified_ReturnsConversationInfoFromApi_AndIsCached()
        {
            var conversationId = "C123";
            var expectedConversation = new Conversation();
            _api.Conversations.Info(conversationId).Returns(expectedConversation);

            _sut.GetConversationById(conversationId)
                .ShouldComplete()
                .And.ShouldBe(expectedConversation);
            _sut.GetConversationById(conversationId)
                .ShouldComplete()
                .And.ShouldBe(expectedConversation);
            _api.Conversations.Received(1).Info(conversationId);
        }

        [Test]
        public void GetConversationByName_FindsChannelWithMatchingName_AndCaches()
        {
            var expectedConversation = new Conversation { Id = "C1", Name = "foo"};
            var otherConversation = new Conversation { Id = "C2", Name = "bar" };
            _api.Conversations.List().Returns(ConversationList(otherConversation, expectedConversation));

            _sut.GetConversationByName("#foo")
                .ShouldComplete()
                .And.ShouldBe(expectedConversation);
            _sut.GetConversationByName("foo")
                .ShouldComplete()
                .And.ShouldBe(expectedConversation);
            _api.Conversations.Received(1).List();
        }

        [Test]
        public void GetConversationByName_UserName_OpensImWithUser_AndCaches()
        {
            var matchingUser = new User { Id = "U1", Name = "foo" };
            var otherUser = new User { Id = "U2", Name = "bar" };
            _api.Users.List().Returns(new UserListResponse { Members = { otherUser, matchingUser } });
            var expectedIm = new Conversation { Id = "D123", User = matchingUser.Id, IsIm = true };
            _api.Conversations.List().Returns(ConversationList());
            _api.Conversations.OpenAndReturnInfo(UserIds(matchingUser.Id)).Returns(new ConversationOpenResponse { Channel = expectedIm });

            _sut.GetConversationByName("@foo")
                .ShouldComplete()
                .And.ShouldBe(expectedIm);
            _sut.GetConversationByName("@foo")
                .ShouldComplete()
                .And.ShouldBe(expectedIm);
            _api.Users.Received(1).List();
            _api.Conversations.ReceivedWithAnyArgs(1).OpenAndReturnInfo(Arg.Any<string[]>());
        }

        [Test]
        public void GetConversationByUserId_OpensImWithUser_AndCaches()
        {
            var expectedIm = new Conversation { Id = "D123", User = "U123", IsIm = true };
            _api.Conversations.List().Returns(ConversationList());
            _api.Conversations.OpenAndReturnInfo(UserIds(expectedIm.User)).Returns(new ConversationOpenResponse { Channel = expectedIm });

            _sut.GetConversationByUserId(expectedIm.User)
                .ShouldComplete()
                .And.ShouldBe(expectedIm);
            _sut.GetConversationByUserId(expectedIm.User)
                .ShouldComplete()
                .And.ShouldBe(expectedIm);
            _api.Conversations.Received(1).OpenAndReturnInfo(UserIds(expectedIm.User));
        }

        [Test]
        public void GetConversations_FetchesConversationList_AndCaches()
        {
            var conversation1 = new Conversation { Id = "C1" };
            var conversation2 = new Conversation { Id = "C2" };
            _api.Conversations.List().Returns(ConversationList(conversation1, conversation2));

            _sut.GetConversations()
                .ShouldComplete()
                .And.ShouldOnlyContain(new[] { conversation1, conversation2 });
            _sut.GetConversations()
                .ShouldComplete()
                .And.ShouldOnlyContain(new[] { conversation1, conversation2 });
            _api.Conversations.Received(1).List();
        }

        [Test]
        public void GetConversations_FetchesAllPages()
        {
            var conversation1 = new Conversation { Id = "C1" };
            var conversation2 = new Conversation { Id = "C2" };
            var page2Cursor = "next cursor";
            _api.Conversations.List().Returns(new ConversationListResponse { Channels = new[] { conversation1 }, ResponseMetadata = new ResponseMetadata { NextCursor = page2Cursor } });
            _api.Conversations.List(cursor: page2Cursor).Returns(new ConversationListResponse { Channels = new[] { conversation2 }, ResponseMetadata = new ResponseMetadata() });

            _sut.GetConversations()
                .ShouldComplete()
                .And.ShouldOnlyContain(new[] { conversation1, conversation2 });
            _sut.GetConversations()
                .ShouldComplete()
                .And.ShouldOnlyContain(new[] { conversation1, conversation2 });
            _api.Conversations.Received(1).List();
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
                Conversation = new Conversation { Id = "channel" },
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
                Conversation = new Conversation { Id = "channel" },
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
                Conversation = new Conversation { Id = "channel" },
                Ts = "123"
            };
            await Connect().ConfigureAwait(false);

            await _sut.Send(new BotMessage { ReplyTo = slackMessage, CreateThread = true }).ConfigureAwait(false);

            await _api.Chat.Received().PostMessage(Arg.Is<Message>(message => message.ThreadTs == slackMessage.Ts), Arg.Any<CancellationToken?>()).ConfigureAwait(false);
        }

        [Test]
        public async Task Send_ReplyInDifferentConversation()
        {
            var slackMessage = new SlackMessage(_sut)
            {
                Conversation = new Conversation { Id = "channel" },
                Ts = "123",
                ThreadTs = "456"
            };
            await Connect().ConfigureAwait(false);

            await _sut.Send(new BotMessage { ReplyTo = slackMessage, Conversation = new Conversation { Id = "other_channel" } }).ConfigureAwait(false);

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

        #region Hubs

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
        public async Task GetHubById_ChannelId_ReturnsChannelInfoFromApi_AndIsCached()
        {
            var channelId = "C123";
            var expectedChannel = new Conversation { Id = channelId, Name = "expected", IsChannel = true };
            _api.Conversations.Info(channelId).Returns(expectedChannel);

            var result = await _sut.GetHubById(channelId);
                
            result.ShouldBeA<Channel>()
                .And.Name.ShouldBe(expectedChannel.Name);
            _sut.GetHubById(channelId)
                .ShouldComplete()
                .And.ShouldBe(result);
            await _api.Conversations.Received(1).Info(channelId);
        }

        [Test]
        public async Task GetHubById_GroupId_ReturnsGroupInfoFromApi_AndIsCached()
        {
            var groupId = "G123";
            var expectedGroup = new Conversation { Id = groupId, Name = "expected", IsGroup = true };
            _api.Conversations.Info(groupId).Returns(expectedGroup);

            var result = await _sut.GetHubById(groupId);

            result.ShouldBeA<Channel>()
                .And.Name.ShouldBe(expectedGroup.Name);
            _sut.GetHubById(groupId)
                .ShouldComplete()
                .And.ShouldBe(result);
            await _api.Conversations.Received(1).Info(groupId);
        }

        [Test]
        public async Task GetHubById_ImId_ReturnsImInfoFromApi_AndIsCached()
        {
            var imId = "D123";
            var expectedIm = new Conversation { Id = imId, User = "expected", IsIm = true };
            _api.Conversations.Info(imId).Returns(expectedIm);

            var result = await _sut.GetHubById(imId);

            result.ShouldBeA<Im>()
                .And.User.ShouldBe(expectedIm.User);
            _sut.GetHubById(imId)
                .ShouldComplete()
                .And.ShouldBe(result);
            await _api.Conversations.Received(1).Info(imId);
        }

        [Test]
        public void GetHubByName_ChannelName_FindsChannelWithMatchingName()
        {
            var expectedChannel = new Conversation { Id = "C1", Name = "foo", IsChannel = true };
            var otherChannel = new Conversation { Id = "C2", Name = "bar", IsChannel = true };
            _api.Conversations.List().Returns(ConversationList(otherChannel, expectedChannel));

            _sut.GetHubByName("#foo")
                .ShouldComplete()
                .And.ShouldBeA<Channel>()
                .And.Name.ShouldBe(expectedChannel.Name);
        }

        [Test]
        public void GetHubByName_UserName_FindsImWithMatchingUser()
        {
            var matchingUser = new User { Id = "U1", Name = "foo" };
            var otherUser = new User { Id = "U2", Name = "bar" };
            _api.Users.List().Returns(new UserListResponse { Members = { otherUser, matchingUser } });
            var expectedIm = new Conversation { Id = "D123", User = matchingUser.Id, IsIm = true };
            _api.Conversations.OpenAndReturnInfo(UserIds(matchingUser.Id)).Returns(new ConversationOpenResponse { Channel = expectedIm });

            _sut.GetHubByName("@foo")
                .ShouldComplete()
                .And.ShouldBeA<Im>()
                .And(im =>
                    {
                        im.Id.ShouldBe(expectedIm.Id);
                        im.User.ShouldBe(expectedIm.User);
                    });
        }

        [Test]
        public void GetHubByName_GroupName_FindsGroupWithMatchingName()
        {
            var expectedGroup = new Conversation { Id = "G1", Name = "foo", IsGroup = true };
            var otherGroup = new Conversation { Id = "G2", Name = "bar", IsGroup = true };
            _api.Conversations.List().Returns(ConversationList(otherGroup, expectedGroup));

            _sut.GetHubByName("foo")
                .ShouldComplete()
                .And.ShouldBeA<Channel>()
                .And.Name.ShouldBe(expectedGroup.Name);
        }

        [Test]
        public async Task GetChannelByName_FindsChannelWithMatchingName_AndCaches()
        {
            var expectedChannel = new Conversation { Id = "C1", Name = "foo", IsChannel = true };
            var otherChannel = new Conversation { Id = "C2", Name = "bar", IsChannel = true };
            _api.Conversations.List().Returns(ConversationList(otherChannel, expectedChannel));

            var result = await _sut.GetChannelByName("#foo");

            result.ShouldBeA<Channel>()
                .And.Name.ShouldBe(expectedChannel.Name);
            _sut.GetChannelByName("foo")
                .ShouldComplete()
                .And.ShouldBe(result);
            await _api.Conversations.Received(1).List();
        }

        [Test]
        public async Task GetImByName_FindsImWithMatchingUserName_AndCaches()
        {
            var matchingUser = new User { Id = "U1", Name = "foo" };
            var otherUser = new User { Id = "U2", Name = "bar" };
            _api.Users.List().Returns(new UserListResponse { Members = { otherUser, matchingUser } });
            var expectedIm = new Conversation { Id = "D123", User = matchingUser.Id, IsIm = true };
            _api.Conversations.OpenAndReturnInfo(UserIds(matchingUser.Id)).Returns(new ConversationOpenResponse { Channel = expectedIm });

            var result = await _sut.GetImByName("@foo");
                
            result.ShouldBeA<Im>()
                .And(im =>
                    {
                        im.Id.ShouldBe(expectedIm.Id);
                        im.User.ShouldBe(expectedIm.User);
                    });
            _sut.GetImByName("foo")
                .ShouldComplete()
                .And.ShouldBe(result);
            await _api.Users.Received(1).List();
            await _api.Conversations.ReceivedWithAnyArgs(1).OpenAndReturnInfo(Arg.Any<string[]>());
        }

        [Test]
        public async Task GetGroupByName_FindsGroupWithMatchingName_AndCaches()
        {
            var expectedGroup = new Conversation { Id = "G1", Name = "foo", IsGroup = true };
            var otherGroup = new Conversation { Id = "G2", Name = "bar", IsGroup = true };
            _api.Conversations.List().Returns(ConversationList(otherGroup, expectedGroup));

            var result = await _sut.GetGroupByName("foo");
            
            result.ShouldBeA<Channel>()
                .And.Name.ShouldBe(expectedGroup.Name);
            _sut.GetGroupByName("foo")
                .ShouldComplete()
                .And.ShouldBe(result);
            await _api.Conversations.Received(1).List();
        }

        [Test]
        public async Task GetImByUserId_OpensImWithUser_AndCaches()
        {
            var expectedIm = new Conversation { Id = "D123", User = "U123", IsIm = true };
            _api.Conversations.OpenAndReturnInfo(UserIds(expectedIm.User)).Returns(new ConversationOpenResponse { Channel = expectedIm });

            var result = await _sut.GetImByUserId(expectedIm.User);
                
            result.ShouldBeA<Im>()
                .And.Id.ShouldBe(expectedIm.Id);
            _sut.GetImByUserId(expectedIm.User)
                .ShouldComplete()
                .And.ShouldBe(result);
            await _api.Conversations.Received(1).OpenAndReturnInfo(UserIds(expectedIm.User));
        }

        [Test]
        public async Task GetChannels_FetchesChannelList_AndCaches()
        {
            var channel1 = new Conversation { Id = "C1", IsChannel = true };
            var channel2 = new Conversation { Id = "C2", IsChannel = true };
            var notAChannel = new Conversation { Id = "D1", IsIm = true };
            _api.Conversations.List().Returns(ConversationList(channel1, channel2, notAChannel));

            var results = await _sut.GetChannels();

            results.ShouldAllBeA<Channel>()
                .And.ShouldOnlyContain(new[] { channel1, channel2 }, (ch, co) => ch.Id == co.Id);
            _sut.GetChannels()
                .ShouldComplete()
                .And.ShouldOnlyContain(results);
            await _api.Conversations.Received(1).List();
        }

        [Test]
        public async Task GetGroups_FetchesGroupList_AndCaches()
        {
            var group1 = new Conversation { Id = "G1", IsGroup = true };
            var group2 = new Conversation { Id = "G2", IsGroup = true };
            var notAGroup = new Conversation { Id = "C1", IsChannel = true };
            _api.Conversations.List().Returns(ConversationList(group1, group2, notAGroup));

            var results = await _sut.GetGroups();
                
            results.ShouldAllBeA<Channel>()
                .And.ShouldOnlyContain(new[] { group1, group2 }, (ch, co) => ch.Id == co.Id);
            _sut.GetGroups()
                .ShouldComplete()
                .And.ShouldOnlyContain(results);
            await _api.Conversations.Received(1).List();
        }

        [Test]
        public async Task GetMpIms_FetchesMpImList_AndCaches()
        {
            var mpim1 = new Conversation { Id = "G1", IsMpim = true };
            var mpim2 = new Conversation { Id = "G2", IsMpim = true };
            var notAnMpim = new Conversation { Id = "C1", IsChannel = true };
            _api.Conversations.List().Returns(ConversationList(mpim1, mpim2, notAnMpim));

            var results = await _sut.GetMpIms();

            results.ShouldAllBeA<Channel>()
                .And.ShouldOnlyContain(new[] { mpim1, mpim2 }, (ch, co) => ch.Id == co.Id);
            _sut.GetMpIms()
                .ShouldComplete()
                .And.ShouldOnlyContain(results);
            await _api.Conversations.Received(1).List();
        }

        [Test]
        public async Task GetIms_FetchesOpenIms_AndCaches()
        {
            var im1 = new Conversation { Id = "D1", IsIm = true };
            var im2 = new Conversation { Id = "D2", IsIm = true };
            var notAnIm = new Conversation { Id = "G1", IsMpim = true };
            _api.Conversations.List().Returns(ConversationList(im1, im2, notAnIm));

            var results = await _sut.GetIms();
                
            results.ShouldAllBeA<Im>()
                .And.ShouldOnlyContain(new[] { im1, im2 }, (im, co) => im.Id == co.Id);
            _sut.GetIms()
                .ShouldComplete()
                .And.ShouldOnlyContain(results);
            await _api.Conversations.Received(1).List();
        }

        #endregion

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

        private static ConversationListResponse ConversationList(params Conversation[] conversations) => 
            new ConversationListResponse { Channels = conversations, ResponseMetadata = new ResponseMetadata() };

        private static string[] UserIds(params string[] userIds) => 
            Arg.Is<string[]>(us => us.SequenceEqual(userIds));
    }
}

