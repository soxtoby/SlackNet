using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SlackNet.Tests.Objects;

public class ConversationTests
{
    private JsonSerializerSettings _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).SerializerSettings;
    }

    [Test]
    public void Conversation_DirectMessage_HasContextTeamIdUpdatedAndIsOpen()
    {
        var result = Deserialize<Conversation>(@"{ ""id"": ""D12345678"", ""created"": 1788600021, ""updated"": 1788600021450, ""is_im"": true, ""is_open"": true, ""context_team_id"": ""T12345678"", ""user"": ""U12345678"" }");

        result.ContextTeamId.ShouldBe("T12345678");
        result.Updated.ShouldBe(1788600021450);
        result.IsOpen.ShouldBe(true);
    }

    [Test]
    public void Conversation_WithoutThoseFields_HasDefaults()
    {
        var result = Deserialize<Conversation>(@"{ ""id"": ""C12345678"", ""is_channel"": true }");

        result.ContextTeamId.ShouldBeNull();
        result.Updated.ShouldBe(0L);
        result.IsOpen.ShouldBe(false);
    }

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}
