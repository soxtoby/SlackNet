using System.Linq;
using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.Events;

namespace SlackNet.Tests;

public class ExtraPropertiesTests
{
    private TestHttp _http;
    private ISlackApiClient _api;
    private JsonSerializerSettings _sut;

    [SetUp]
    public void SetUp()
    {
        var jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
        _sut = jsonSettings.SerializerSettings;
        _http = new TestHttp();
        _api = new SlackApiClient(_http, Default.UrlBuilder(jsonSettings), jsonSettings, "token");
    }

    [Test]
    public async Task User_PropertiesSlackNetDoesNotKnowAbout_AreInExtraProperties()
    {
        _http.Response = JObject.Parse(@"{
            ""ok"": true,
            ""user"": {
                ""id"": ""U12345678"",
                ""a_flag_slack_adds_later"": true,
                ""profile"": { ""email"": ""ada@example.com"", ""a_string_slack_adds_later"": ""foo"" },
                ""enterprise_user"": { ""id"": ""W12345678"", ""a_number_slack_adds_later"": 42 }
            }
        }");

        var user = await _api.Users.Info("U12345678");

        user.Id.ShouldBe("U12345678");
        user.ExtraProperties["a_flag_slack_adds_later"].ShouldBe(true);
        user.Profile.Email.ShouldBe("ada@example.com");
        user.Profile.ExtraProperties["a_string_slack_adds_later"].ShouldBe("foo");
        user.EnterpriseUser.ExtraProperties["a_number_slack_adds_later"].ShouldBe(42L);
    }

    [Test]
    public async Task User_WithOnlyPropertiesSlackNetKnows_HasNoExtraProperties()
    {
        _http.Response = JObject.Parse(@"{ ""ok"": true, ""user"": { ""id"": ""U12345678"", ""is_admin"": true, ""profile"": { ""email"": ""ada@example.com"" } } }");

        var user = await _api.Users.Info("U12345678");

        user.ExtraProperties.ShouldBeEmpty();
        user.Profile.ExtraProperties.ShouldBeEmpty();
    }

    [Test]
    public async Task Conversation_PropertiesSlackNetDoesNotKnowAbout_AreInExtraProperties()
    {
        _http.Response = JObject.Parse(@"{
            ""ok"": true,
            ""channel"": {
                ""id"": ""C12345678"",
                ""a_flag_slack_adds_later"": true,
                ""topic"": { ""value"": ""Hello"", ""a_string_slack_adds_later"": ""foo"" }
            }
        }");

        var conversation = await _api.Conversations.Info("C12345678");

        conversation.ExtraProperties["a_flag_slack_adds_later"].ShouldBe(true);
        conversation.Topic.ExtraProperties["a_string_slack_adds_later"].ShouldBe("foo");
    }

    [Test]
    public void Block_PropertiesSlackNetDoesNotKnowAbout_AreInExtraProperties()
    {
        var result = Deserialize<Event>(@"{
            ""type"": ""message"",
            ""blocks"": [
                {
                    ""type"": ""section"",
                    ""text"": { ""type"": ""mrkdwn"", ""text"": ""Hello"", ""a_flag_slack_adds_later"": true },
                    ""a_string_slack_adds_later"": ""foo""
                }
            ]
        }");

        var section = result.ShouldBeA<MessageEvent>().And.Blocks.Single().ShouldBeA<SectionBlock>().And;
        section.ExtraProperties["a_string_slack_adds_later"].ShouldBe("foo");
        section.Text.ExtraProperties["a_flag_slack_adds_later"].ShouldBe(true);
    }

    [Test]
    public void ExtraProperties_AreNotSentBackToSlack()
    {
        var block = Deserialize<Block>(@"{ ""type"": ""section"", ""text"": { ""type"": ""plain_text"", ""text"": ""Hello"" }, ""a_string_slack_adds_later"": ""foo"" }");
        block.ExtraProperties.Keys.ShouldContain("a_string_slack_adds_later");

        var json = JObject.Parse(JsonConvert.SerializeObject(block, _sut));

        json.Value<string>("type").ShouldBe("section");
        json.ContainsKey("a_string_slack_adds_later").ShouldBe(false);
    }

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}
