using System.Collections.Generic;
using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SlackNet.Events;

namespace SlackNet.Tests
{
    public class SerializationTests
    {
        private JsonSerializerSettings _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).SerializerSettings;
        }

        [Test]
        public void SnakeCaseJson_DeserializedToPascalCase()
        {
            var result = JsonConvert.DeserializeObject<SimpleType>(@"{ ""some_property"": ""foo"" }", _sut);
            result.SomeProperty.ShouldBe("foo");
        }

        [Test]
        public void PascalCaseProperties_SerializedAsSnakeCase()
        {
            var result = JsonConvert.SerializeObject(new SimpleType { SomeProperty = "foo" }, _sut);
            result.ShouldBe(@"{""some_property"":""foo""}");
        }

        [Test]
        public void EnumValues_DeserializedFromSnakeCase()
        {
            var result = JsonConvert.DeserializeObject<EnumProperty>(@"{ ""property"": ""test_value"" }",_sut);
            result.Property.ShouldBe(TestEnum.TestValue);
        }

        [Test]
        public void EnumValues_SerializedToSnakeCase()
        {
            var result = JsonConvert.SerializeObject(new EnumProperty { Property = TestEnum.TestValue }, _sut);
            result.ShouldBe(@"{""property"":""test_value""}");
        }

        [Test]
        public void SlackTypesCanBeDeserialized()
        {
            var result = JsonConvert.DeserializeObject<Event>(@"{ ""type"": ""message"", ""text"": ""foo"" }", _sut);
            result.ShouldBeA<MessageEvent>()
                .And.Text.ShouldBe("foo");
        }

        [Test]
        public void SlackSubTypesCanBeDeserialized()
        {
            var result = JsonConvert.DeserializeObject<Event>(@"{ ""type"": ""message"", ""subtype"": ""bot_message"", ""text"": ""foo"" }", _sut);
            result.ShouldBeA<BotMessage>()
                .And.Text.ShouldBe("foo");
        }

        [Test]
        public void SlackTypePropertyCanBeDeserialized()
        {
            var result = JsonConvert.DeserializeObject<HasSlackTypeProperty>(@"{ ""event"": { ""type"": ""message"", ""text"": ""foo"" } }", _sut);
            result.Event.ShouldBeA<MessageEvent>()
                .And.Text.ShouldBe("foo");
        }

        [Test]
        public void SlackTypesInArrayCanBeDeserialized()
        {
            var result = JsonConvert.DeserializeObject<HasSlackTypeArray>(@"{ ""event_list"": [{ ""type"": ""message"", ""text"": ""foo"" }] }", _sut);
            result.EventList.ShouldBeASingular<MessageEvent>()
                .And.Text.ShouldBe("foo");
        }

        [Test]
        public void Icons()
        {
            var result = JsonConvert.DeserializeObject<Icons>(@"{ ""icon_32"": ""foo"", ""icon_64"": ""bar"" }");
            result.Images["icon_32"].ShouldBe("foo");
            result.Images["icon_64"].ShouldBe("bar");
        }

        class SimpleType
        {
            public string SomeProperty { get; set; }
        }

        class EnumProperty
        {
            public TestEnum Property { get; set; }
        }

        enum TestEnum
        {
            Default,
            TestValue
        }

        class HasSlackTypeProperty
        {
            public Event Event { get; set; }
            public IList<Event> EventList { get; set; }
        }

        class HasSlackTypeArray
        {
            public Event Event { get; set; }
            public IList<Event> EventList { get; set; }
        }
    }
}
