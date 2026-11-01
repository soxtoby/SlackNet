using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SlackNet.Events;

namespace SlackNet.Tests;

public class MessageThreadTests
{
    private JsonSerializerSettings _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).SerializerSettings;
    }

    [Test]
    public void MessageEvent_ThreadReply_HasParentUserId()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""message"", ""user"": ""U123ABC456"", ""text"": ""one island"", ""thread_ts"": ""1482960137.003543"", ""parent_user_id"": ""U061F7AUR"", ""ts"": ""1483037603.017503"" }");

        var message = result.ShouldBeA<MessageEvent>().And;
        message.ParentUserId.ShouldBe("U061F7AUR");
        message.ExtraProperties.ShouldBeEmpty();
    }

    [Test]
    public void MessageEvent_ThreadParent_HasSubscribed()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""message"", ""user"": ""U123ABC456"", ""text"": ""island"", ""thread_ts"": ""1482960137.003543"", ""reply_count"": 3, ""subscribed"": true, ""ts"": ""1482960137.003543"" }");

        var message = result.ShouldBeA<MessageEvent>().And;
        message.Subscribed.ShouldBe(true);
        message.ExtraProperties.ShouldBeEmpty();
    }

    [Test]
    public void MessageEvent_OutsideAThread_HasNoParentUserIdAndIsNotSubscribed()
    {
        var result = Deserialize<Event>(@"{ ""type"": ""message"", ""user"": ""U123ABC456"", ""text"": ""Hello"", ""ts"": ""1355517523.000005"" }");

        var message = result.ShouldBeA<MessageEvent>().And;
        message.ParentUserId.ShouldBeNull();
        message.Subscribed.ShouldBe(false);
    }

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}
