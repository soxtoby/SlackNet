using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SlackNet.Events;

namespace SlackNet.Tests;

public class MemberChannelEventTests
{
    private JsonSerializerSettings _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).SerializerSettings;
    }

    [Test]
    public void MemberJoinedChannel_HasTeamAndEnterprise()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""member_joined_channel"", ""user"": ""W123ABC456"", ""channel"": ""C123ABC456"", ""channel_type"": ""C"", ""team"": ""T123ABC456"", ""inviter"": ""U123456789"", ""enterprise"": ""E123456789"" }");

        var joined = result.ShouldBeA<MemberJoinedChannel>().And;
        joined.Team.ShouldBe("T123ABC456");
        joined.Enterprise.ShouldBe("E123456789");
        joined.ExtraProperties.ShouldBeEmpty();
    }

    [Test]
    public void MemberJoinedChannel_WithoutTeamOrEnterprise_HasNeither()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""member_joined_channel"", ""user"": ""W123ABC456"", ""channel"": ""C123ABC456"", ""channel_type"": ""C"" }");

        var joined = result.ShouldBeA<MemberJoinedChannel>().And;
        joined.Team.ShouldBeNull();
        joined.Enterprise.ShouldBeNull();
    }

    [Test]
    public void MemberLeftChannel_HasTeam()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""member_left_channel"", ""user"": ""W123ABC456"", ""channel"": ""C123ABC456"", ""channel_type"": ""C"", ""team"": ""T123ABC456"" }");

        var left = result.ShouldBeA<MemberLeftChannel>().And;
        left.Team.ShouldBe("T123ABC456");
        left.ExtraProperties.ShouldBeEmpty();
    }

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}
