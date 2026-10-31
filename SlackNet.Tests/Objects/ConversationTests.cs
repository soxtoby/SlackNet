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

    [Test]
    public void Conversation_SharedChannel_HasFrozenStateAndTeamIds()
    {
        var result = Deserialize<Conversation>(@"{ ""id"": ""C12345678"", ""is_ext_shared"": true, ""is_frozen"": true, ""shared_team_ids"": [""T111"", ""T222""], ""internal_team_ids"": [""T111""], ""connected_team_ids"": [""T333""], ""pending_connected_team_ids"": [""T444""] }");

        result.IsFrozen.ShouldBe(true);
        result.SharedTeamIds.ShouldMatch(["T111", "T222"]);
        result.InternalTeamIds.ShouldMatch(["T111"]);
        result.ConnectedTeamIds.ShouldMatch(["T333"]);
        result.PendingConnectedTeamIds.ShouldMatch(["T444"]);
    }

    [Test]
    public void Conversation_NotShared_HasNoTeamIds()
    {
        var result = Deserialize<Conversation>(@"{ ""id"": ""C12345678"", ""is_channel"": true }");

        result.IsFrozen.ShouldBe(false);
        result.SharedTeamIds.ShouldBeEmpty();
        result.InternalTeamIds.ShouldBeEmpty();
        result.ConnectedTeamIds.ShouldBeEmpty();
        result.PendingConnectedTeamIds.ShouldBeEmpty();
    }

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}
