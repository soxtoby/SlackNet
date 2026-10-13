using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SlackNet.Events;

namespace SlackNet.Tests;

public class MessageTeamTests
{
    private JsonSerializerSettings _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).SerializerSettings;
    }

    [Test]
    public void MessageEvent_FromSharedChannel_HasUserTeamAndSourceTeam()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""message"", ""user"": ""U123ABC456"", ""text"": ""Hello"", ""ts"": ""1355517523.000005"", ""team"": ""T111"", ""user_team"": ""T222"", ""source_team"": ""T333"" }");

        var message = result.ShouldBeA<MessageEvent>().And;
        message.Team.ShouldBe("T111");
        message.UserTeam.ShouldBe("T222");
        message.SourceTeam.ShouldBe("T333");
        message.ExtraProperties.ShouldBeEmpty();
    }

    [Test]
    public void MessageEvent_FromUnsharedChannel_HasNoUserTeamOrSourceTeam()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""message"", ""user"": ""U123ABC456"", ""text"": ""Hello"", ""ts"": ""1355517523.000005"", ""team"": ""T111"" }");

        var message = result.ShouldBeA<MessageEvent>().And;
        message.UserTeam.ShouldBeNull();
        message.SourceTeam.ShouldBeNull();
    }

    [Test]
    public void MessageChanged_MessagesHaveUserTeamAndSourceTeam()
    {
        var result = Deserialize<Event>(@"{
            ""type"": ""message"",
            ""subtype"": ""message_changed"",
            ""channel"": ""C123ABC456"",
            ""message"": { ""type"": ""message"", ""user"": ""U123ABC456"", ""text"": ""Hello again"", ""ts"": ""1355517523.000005"", ""user_team"": ""T222"", ""source_team"": ""T333"" },
            ""previous_message"": { ""type"": ""message"", ""user"": ""U123ABC456"", ""text"": ""Hello"", ""ts"": ""1355517523.000005"", ""user_team"": ""T222"", ""source_team"": ""T333"" }
        }");

        var changed = result.ShouldBeA<MessageChanged>().And;
        changed.Message.UserTeam.ShouldBe("T222");
        changed.PreviousMessage.SourceTeam.ShouldBe("T333");
    }

    [Test]
    public void AppMention_FromSharedChannel_HasUserTeamAndSourceTeam()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""app_mention"", ""user"": ""U123ABC456"", ""text"": ""<@U0LAN0Z89> hi"", ""ts"": ""1355517523.000005"", ""user_team"": ""T222"", ""source_team"": ""T333"" }");

        var mention = result.ShouldBeA<AppMention>().And;
        mention.UserTeam.ShouldBe("T222");
        mention.SourceTeam.ShouldBe("T333");
    }

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}
