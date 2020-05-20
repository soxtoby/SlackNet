using EasyAssertions;
using NUnit.Framework;
using SlackNet.WebApi;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.Tests
{
    public class SlackUrlBuilderTests
    {
        private const string BaseUrl = "https://slack.com/api/";
        private ISlackUrlBuilder _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = Default.UrlBuilder(Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)));
        }

        [Test]
        public void BasicUrlWithoutArgs()
        {
            _sut.Url("some.method", new Args())
                .ShouldBe(BaseUrl + "some.method");
        }

        [Test]
        public void StringArgs_Inline()
        {
            _sut.Url("method", new Args { { "foo", "bar" } })
                .ShouldBe(BaseUrl + "method?foo=bar");
        }

        [Test]
        public void StringArgs_Escaped()
        {
            _sut.Url("method", new Args { { "foo", "?" } })
                .ShouldBe(BaseUrl + "method?foo=%3F");
        }

        [Test]
        public void StringListArgs_SingleItem_Inline()
        {
            _sut.Url("method", new Args { { "foo", new[] { "bar" } } })
                .ShouldBe(BaseUrl + "method?foo=bar");
        }

        [Test]
        public void StringListArgs_CommaSeparatedAndEscaped()
        {
            _sut.Url("method", new Args { { "foo", new[] { "?bar", "baz" } } })
                .ShouldBe(BaseUrl + "method?foo=%3Fbar%2Cbaz");
        }

        [Test]
        public void EnumListArgs_CommaSeparatedEscaped()
        {
            _sut.Url("method", new Args { { "foo", new[] { ConversationType.PrivateChannel, ConversationType.Im } } })
                .ShouldBe(BaseUrl + "method?foo=private_channel%2Cim");
        }

        [Test]
        public void Objects_SerializedAndEscaped()
        {
            _sut.Url("method", new Args { { "foo", new SomeObject { SomeProperty = "bar" } } })
                .ShouldBe(BaseUrl + "method?foo=%7B%22some_property%22%3A%22bar%22%7D");
        }

        [Test]
        public void NullArgs_Ignored()
        {
            _sut.Url("method", new Args { { "foo", null }, { "bar", "baz" } })
                .ShouldBe(BaseUrl + "method?bar=baz");
        }

        class SomeObject
        {
            public string SomeProperty { get; set; }
        }
    }
}