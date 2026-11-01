using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SlackNet.Events;

namespace SlackNet.Tests;

public class EventCallbackTests
{
    private JsonSerializerSettings _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).SerializerSettings;
    }

    [Test]
    public void EventCallback_HasContextTeamIdAndContextEnterpriseId()
    {
        var result = Deserialize<EventCallback>(@"{
            ""type"": ""event_callback"",
            ""team_id"": ""T111"",
            ""api_app_id"": ""A123ABC456"",
            ""event"": { ""type"": ""app_mention"", ""user"": ""U123ABC456"", ""text"": ""<@U0LAN0Z89> hi"", ""ts"": ""1355517523.000005"" },
            ""event_id"": ""Ev123ABC456"",
            ""event_time"": 1355517523,
            ""is_ext_shared_channel"": true,
            ""context_team_id"": ""T222"",
            ""context_enterprise_id"": ""E333""
        }");

        result.TeamId.ShouldBe("T111");
        result.ContextTeamId.ShouldBe("T222");
        result.ContextEnterpriseId.ShouldBe("E333");
    }

    [Test]
    public void EventCallback_WithNullContextEnterpriseId_HasNone()
    {
        var result = Deserialize<EventCallback>(@"{ ""type"": ""event_callback"", ""team_id"": ""T111"", ""context_team_id"": ""T111"", ""context_enterprise_id"": null }");

        result.ContextTeamId.ShouldBe("T111");
        result.ContextEnterpriseId.ShouldBeNull();
    }

    [Test]
    public void Authorization_ForOrgWideInstall_IsEnterpriseInstall()
    {
        var result = Deserialize<EventCallback>(@"{
            ""type"": ""event_callback"",
            ""authorizations"": [
                { ""enterprise_id"": ""E123ABC456"", ""team_id"": null, ""user_id"": ""U123ABC456"", ""is_bot"": true, ""is_enterprise_install"": true },
                { ""enterprise_id"": ""E123ABC456"", ""team_id"": ""T123ABC456"", ""user_id"": ""U234ABC567"", ""is_bot"": false, ""is_enterprise_install"": false }
            ]
        }");

        result.Authorizations[0].IsEnterpriseInstall.ShouldBe(true);
        result.Authorizations[1].IsEnterpriseInstall.ShouldBe(false);
    }

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}
