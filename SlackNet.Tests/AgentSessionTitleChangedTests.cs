using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SlackNet.Events;

namespace SlackNet.Tests;

public class AgentSessionTitleChangedTests
{
    private JsonSerializerSettings _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).SerializerSettings;
    }

    [Test]
    public void AgentSessionTitleChanged_DeserializedFromSlackPayload()
    {
        var result = Deserialize<Event>(@"{ ""channel"": ""C0123ABC456"", ""event_ts"": ""1783536983.783769"", ""previous_title"": ""Scuba diving research"", ""team_id"": ""T0123ABC456"", ""thread_ts"": ""1782234671.392669"", ""title"": ""Bora Bora trip prep"", ""type"": ""agent_session_title_changed"", ""user"": ""U123ABC456"" }");

        var changed = result.ShouldBeA<AgentSessionTitleChanged>().And;
        changed.Channel.ShouldBe("C0123ABC456");
        changed.ThreadTs.ShouldBe("1782234671.392669");
        changed.User.ShouldBe("U123ABC456");
        changed.Title.ShouldBe("Bora Bora trip prep");
        changed.PreviousTitle.ShouldBe("Scuba diving research");
        changed.EventTs.ShouldBe("1783536983.783769");
    }

    [Test]
    public void AgentSessionTitleChanged_SessionHadNoTitle_HasNoPreviousTitle()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""agent_session_title_changed"", ""channel"": ""C1"", ""thread_ts"": ""1.2"", ""user"": ""U1"", ""title"": ""First title"" }");

        var changed = result.ShouldBeA<AgentSessionTitleChanged>().And;
        changed.Title.ShouldBe("First title");
        changed.PreviousTitle.ShouldBeNull();
    }

    [Test]
    public void AgentSessionTitleChanged_InSessionChannel_MayHaveNoThread()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""agent_session_title_changed"", ""channel"": ""C1"", ""user"": ""U1"", ""title"": ""Renamed"" }");

        result.ShouldBeA<AgentSessionTitleChanged>().And.ThreadTs.ShouldBeNull();
    }

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}
