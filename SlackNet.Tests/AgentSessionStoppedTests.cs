using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SlackNet.Events;

namespace SlackNet.Tests;

public class AgentSessionStoppedTests
{
    private JsonSerializerSettings _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).SerializerSettings;
    }

    [Test]
    public void AgentSessionStopped_DeserializedFromSlackPayload()
    {
        var result = Deserialize<Event>(@"{ ""channel"": ""C0123ABC456"", ""event_ts"": ""1783536983.783769"", ""streaming_message_ts"": [""1782234987.693923""], ""thread_ts"": ""1782234671.392669"", ""type"": ""agent_session_stopped"", ""user"": ""U123ABC456"" }");

        var stopped = result.ShouldBeA<AgentSessionStopped>().And;
        stopped.Channel.ShouldBe("C0123ABC456");
        stopped.ThreadTs.ShouldBe("1782234671.392669");
        stopped.User.ShouldBe("U123ABC456");
        stopped.StreamingMessageTs.ShouldMatch(["1782234987.693923"]);
        stopped.EventTs.ShouldBe("1783536983.783769");
        stopped.ExtraProperties.ShouldBeEmpty();
    }

    [Test]
    public void AgentSessionStopped_NoActiveStream_HasEmptyStreamingMessageTs()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""agent_session_stopped"", ""channel"": ""C1"", ""thread_ts"": ""1.2"", ""user"": ""U1"", ""streaming_message_ts"": [] }");

        result.ShouldBeA<AgentSessionStopped>().And.StreamingMessageTs.ShouldBeEmpty();
    }

    [Test]
    public void AgentSessionStopped_InSessionChannel_MayHaveNoThread()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""agent_session_stopped"", ""channel"": ""C1"", ""user"": ""U1"", ""streaming_message_ts"": [] }");

        result.ShouldBeA<AgentSessionStopped>().And.ThreadTs.ShouldBeNull();
    }

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}