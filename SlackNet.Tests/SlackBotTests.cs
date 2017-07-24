using NSubstitute;
using NUnit.Framework;
using SlackNet.Bot;
using SlackNet.WebApi;

namespace SlackNet.Tests
{
    [Ignore("Need to inject scheduler")]
    public class SlackBotTests
    {
        private SlackBot _sut;
        private ISlackRtmClient _rtm;
        private ISlackApiClient _api;

        [SetUp]
        public void SetUp()
        {
            _rtm = Substitute.For<ISlackRtmClient>();
            _api = Substitute.For<ISlackApiClient>();
            _sut = new SlackBot(_rtm, _api);
        }

        [Test]
        public void Send_ReplyInChannel()
        {
            SlackMessage slackMessage = new SlackMessage(_sut)
                {
                    Hub = new Channel { Id = "channel" },
                    Ts = "123"
                };

            _sut.Send(new BotMessage { ReplyTo = slackMessage, CreateThread = false });

            _api.Chat.Received().PostMessage(Arg.Is<Message>(message => message.ThreadTs == null));
        }

        [Test]
        public void Send_ReplyInExistingThread()
        {
            SlackMessage slackMessage = new SlackMessage(_sut)
                {
                    Hub = new Channel { Id = "channel" },
                    Ts = "123",
                    ThreadTs = "456"
                };

            _sut.Send(new BotMessage { ReplyTo = slackMessage, CreateThread = false });

            _api.Chat.Received().PostMessage(Arg.Is<Message>(message => message.ThreadTs == slackMessage.ThreadTs));
        }

        [Test]
        public void Send_ReplyInNewThread()
        {
            SlackMessage slackMessage = new SlackMessage(_sut)
                {
                    Hub = new Channel { Id = "channel" },
                    Ts = "123"
                };

            _sut.Send(new BotMessage { ReplyTo = slackMessage, CreateThread = true });

            _api.Chat.Received().PostMessage(Arg.Is<Message>(message => message.ThreadTs == slackMessage.Ts));
        }

        [Test]
        public void Send_ReplyInDifferentHub()
        {
            SlackMessage slackMessage = new SlackMessage(_sut)
                {
                    Hub = new Channel { Id = "channel" },
                    Ts = "123",
                    ThreadTs = "456"
                };

            _sut.Send(new BotMessage { ReplyTo = slackMessage, Hub = new HubByRef(new Channel { Id = "other_channel" }) });

            _api.Chat.Received().PostMessage(Arg.Is<Message>(message => message.ThreadTs == null && message.Channel == "other_channel"));
        }
    }
}
