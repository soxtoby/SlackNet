using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
            var result = Deserialize<SimpleType>(@"{ ""some_property"": ""foo"" }");
            result.SomeProperty.ShouldBe("foo");
        }

        [Test]
        public void PascalCaseProperties_SerializedAsSnakeCase()
        {
            var result = Serialize(new SimpleType { SomeProperty = "foo" });
            result.ShouldBe(@"{""some_property"":""foo""}");
        }

        [Test]
        public void EnumValues_DeserializedFromSnakeCase()
        {
            var result = Deserialize<EnumProperty>(@"{ ""property"": ""test_value"" }");
            result.Property.ShouldBe(TestEnum.TestValue);
        }

        [Test]
        public void EnumValues_SerializedToSnakeCase()
        {
            var result = Serialize(new EnumProperty { Property = TestEnum.TestValue });
            result.ShouldBe(@"{""property"":""test_value""}");
        }

        [Test]
        public void EnumValues_WithEnumMemberAttribute_DeserializedFromAttributeValue()
        {
            var result = Deserialize<EnumProperty>(@"{ ""property"": ""other_value"" }");
            result.Property.ShouldBe(TestEnum.RenamedValue);
        }

        [Test]
        public void EnumValues_WithEnumMemberAttribute_SerializedToAttributeValue()
        {
            var result = Serialize(new EnumProperty { Property = TestEnum.RenamedValue });
            result.ShouldBe(@"{""property"":""other_value""}");
        }

        [Test]
        public void SlackTypesCanBeDeserialized()
        {
            var result = Deserialize<Event>(@"{ ""type"": ""message"", ""text"": ""foo"" }");
            result.ShouldBeA<MessageEvent>()
                .And.Text.ShouldBe("foo");
        }

        [Test]
        public void SlackSubTypesCanBeDeserialized()
        {
            var result = Deserialize<Event>(@"{ ""type"": ""message"", ""subtype"": ""bot_message"", ""text"": ""foo"" }");
            result.ShouldBeA<BotMessage>()
                .And.Text.ShouldBe("foo");
        }

        [Test]
        public void SlackTypePropertyCanBeDeserialized()
        {
            var result = Deserialize<HasSlackTypeProperty>(@"{ ""event"": { ""type"": ""message"", ""text"": ""foo"" } }");
            result.Event.ShouldBeA<MessageEvent>()
                .And.Text.ShouldBe("foo");
        }

        [Test]
        public void SlackTypesInArrayCanBeDeserialized()
        {
            var result = Deserialize<HasSlackTypeArray>(@"{ ""event_list"": [{ ""type"": ""message"", ""text"": ""foo"" }] }");
            result.EventList.ShouldBeASingular<MessageEvent>()
                .And.Text.ShouldBe("foo");
        }

        [Test]
        public void Icons()
        {
            var result = Deserialize<Icons>(@"{ ""icon_32"": ""foo"", ""icon_64"": ""bar"" }");
            result.Images["icon_32"].ShouldBe("foo");
            result.Images["icon_64"].ShouldBe("bar");
        }

        [Test]
        public void IgnoreIfEmpty_IsEmpty_Ignored()
        {
            var result = Serialize(new IgnoreIfEmptyProperty { List = new List<string>() });
            result.ShouldBe("{}");
        }

        [Test]
        public void IgnoreIfEmpty_NotEmpty_Serialized()
        {
            var result = Serialize(new IgnoreIfEmptyProperty { List = new List<string>{ "foo" } });
            result.ShouldBe(@"{""list"":[""foo""]}");
        }

        [Test]
        public void IgnoreIfDefault_IsDefault_Ignored()
        {
            var result = Serialize(new IgnoreIfDefaultProperty { Value = TestEnum.Default });
            result.ShouldBe("{}");
        }

        [Test]
        public void IgnoreIfDefault_NotDefault_Serialized()
        {
            var result = Serialize(new IgnoreIfDefaultProperty { Value = TestEnum.TestValue });
            result.ShouldBe(@"{""value"":""test_value""}");
        }

        [Test]
        public void UserProfileFields_EmptyArray_IsEmpty()
        {
            var result = Deserialize<UserProfile>(@"{""fields"":[],""phone"":""123""}");
            result.Fields.ShouldBeEmpty();
            result.Phone.ShouldBe("123");
        }

        [Test]
        public void UserProfileFields_PopulatedObject_IsPopulated()
        {
            var result = Deserialize<UserProfile>(@"{""fields"":{""fieldId"":{""value"":""foo""}},""phone"":""123""}");
            result.Fields.Keys.ShouldMatch(new[] { "fieldId" });
            result.Fields["fieldId"].Value.ShouldBe("foo");
            result.Phone.ShouldBe("123");
        }

        [Test]
        public void DateTime_SerializedAsDateString()
        {
            Serialize(new HasDateTimeProperty { Required = new DateTime(2001, 2, 3) })
                .ShouldBe(@"{""required"":""2001-02-03""}");

            Serialize(new HasDateTimeProperty { Required = new DateTime(2001, 2, 3), Optional = new DateTime(2004, 5, 6) })
                .ShouldBe(@"{""required"":""2001-02-03"",""optional"":""2004-05-06""}");
        }

        [Test]
        public void DateTime_DeserializedFromDateString()
        {
            Deserialize<HasDateTimeProperty>(@"{""required"":""2001-02-03""}")
                .Assert(o =>
                {
                    o.Required.ShouldBe(new DateTime(2001, 2, 3));
                    o.Optional.ShouldBeNull();
                });

            Deserialize<HasDateTimeProperty>(@"{""required"":""2001-02-03"",""optional"":""2004-05-06""}")
                .Assert(o =>
                {
                    o.Required.ShouldBe(new DateTime(2001, 2, 3));
                    o.Optional.ShouldBe(new DateTime(2004, 5, 6));
                });
        }

        private string Serialize(object obj) => JsonConvert.SerializeObject(obj, _sut);

        private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);

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
            TestValue,
            [EnumMember(Value = "other_value")]
            RenamedValue
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

        class IgnoreIfEmptyProperty
        {
            [IgnoreIfEmpty]
            public IList<string> List { get; set; }
        }

        class IgnoreIfDefaultProperty
        {
            [IgnoreIfDefault]
            public TestEnum Value { get; set; }
        }

        class HasDateTimeProperty
        {
            public DateTime Required { get; set; }
            public DateTime? Optional { get; set; }
        }
    }
}
