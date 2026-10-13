using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SlackNet.Events;

namespace SlackNet.Tests;

public class MessageDeletedTests
{
    private JsonSerializerSettings _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).SerializerSettings;
    }

    [Test]
    public void MessageDeleted_DeserializedWithPreviousMessage()
    {
        var result = Deserialize<Event>(@"{
            ""type"": ""message"",
            ""subtype"": ""message_deleted"",
            ""hidden"": true,
            ""channel"": ""C123ABC456"",
            ""ts"": ""1358878755.000001"",
            ""deleted_ts"": ""1358878749.000002"",
            ""event_ts"": ""1358878755.000001"",
            ""previous_message"": { ""type"": ""message"", ""user"": ""U123ABC456"", ""text"": ""Hello"", ""ts"": ""1358878749.000002"", ""thread_ts"": ""1358878700.000001"" }
        }");

        var deleted = result.ShouldBeA<MessageDeleted>().And;
        deleted.DeletedTs.ShouldBe("1358878749.000002");
        deleted.PreviousMessage.User.ShouldBe("U123ABC456");
        deleted.PreviousMessage.Text.ShouldBe("Hello");
        deleted.PreviousMessage.Ts.ShouldBe("1358878749.000002");
        deleted.PreviousMessage.ThreadTs.ShouldBe("1358878700.000001");
        deleted.ExtraProperties.ContainsKey("previous_message").ShouldBe(false);
    }

    [Test]
    public void MessageDeleted_WithoutPreviousMessage_HasNone()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""message"", ""subtype"": ""message_deleted"", ""hidden"": true, ""channel"": ""C123ABC456"", ""ts"": ""1358878755.000001"", ""deleted_ts"": ""1358878749.000002"" }");

        result.ShouldBeA<MessageDeleted>().And.PreviousMessage.ShouldBeNull();
    }

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}
