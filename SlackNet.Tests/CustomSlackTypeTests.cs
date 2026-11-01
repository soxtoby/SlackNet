using System;
using System.Linq;
using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.Events;

namespace SlackNet.Tests;

public class CustomSlackTypeTests
{
    [Test]
    public void ReplaceSlackType_TypeIdentifiedBySlackType_DeserializedAsReplacement()
    {
        var sut = new SlackServiceBuilder().ReplaceSlackType<SectionBlock, CustomSectionBlock>();

        var result = Deserialize<Block>(sut, @"{ ""type"": ""section"", ""not_in_slack_net"": ""value"" }");

        result.ShouldBeA<CustomSectionBlock>()
            .And.NotInSlackNet.ShouldBe("value");
    }

    [Test]
    public void ReplaceSlackType_TypeWithSubtypes_SubtypesDeserializedAsSlackNetTypes()
    {
        var sut = new SlackServiceBuilder().ReplaceSlackType<MessageEvent, CustomMessageEvent>();

        var result = Deserialize<Event[]>(sut, @"[{ ""type"": ""message"" }, { ""type"": ""message"", ""subtype"": ""bot_message"" }]");

        result.Select(e => e.GetType()).ShouldMatch([typeof(CustomMessageEvent), typeof(BotMessage)]);
    }

    [Test]
    public void RegisterSlackType_NewTypeIdentifier_DeserializedAsRegisteredType()
    {
        var sut = new SlackServiceBuilder().RegisterSlackType<CustomEvent>("new_event_type");

        var result = Deserialize<Event>(sut, @"{ ""type"": ""new_event_type"", ""not_in_slack_net"": ""value"" }");

        result.ShouldBeA<CustomEvent>()
            .And.NotInSlackNet.ShouldBe("value");
    }

    [Test]
    public void RegisterSlackType_MessageSubtypeSharingIdentifierWithEvent_EventStillDeserializedAsSlackNetType()
    {
        var sut = new SlackServiceBuilder().RegisterSlackType<CustomMessageEvent>("channel_created");

        var result = Deserialize<Event[]>(sut, @"[{ ""type"": ""channel_created"" }, { ""type"": ""message"", ""subtype"": ""channel_created"" }]");

        result.Select(e => e.GetType()).ShouldMatch([typeof(ChannelCreated), typeof(CustomMessageEvent)]);
    }

    [Test]
    public void RegisterSlackType_IdentifierSlackNetAlreadyHas_Throws()
    {
        Should.Throw<ArgumentException>(() => new SlackServiceBuilder().RegisterSlackType<CustomBotMessage>("bot_message"))
            .And.ParamName.ShouldBe("typeIdentifier");
    }

    [Test]
    public void RegisterSlackType_CustomBaseClassAndIdentifierSlackNetAlreadyHas_Throws()
    {
        Should.Throw<ArgumentException>(() => new SlackServiceBuilder().RegisterSlackType<CustomChannelCreated>("channel_created"))
            .And.ParamName.ShouldBe("typeIdentifier");
    }

    private static T Deserialize<T>(ISlackServiceProvider services, string json) =>
        JsonConvert.DeserializeObject<T>(json, services.GetJsonSettings().SerializerSettings);

    public class CustomSectionBlock : SectionBlock
    {
        public string NotInSlackNet { get; set; }
    }

    public class CustomMessageEvent : MessageEvent { }

    public class CustomEvent : Event
    {
        public string NotInSlackNet { get; set; }
    }

    public class CustomBotMessage : BotMessage { }

    public abstract class CustomEventBase : Event { }

    public class CustomChannelCreated : CustomEventBase { }
}
