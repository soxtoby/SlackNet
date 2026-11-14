using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EasyAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace SlackNet.Tests;

public class WebApiErrorResponseTests
{
    private TestHttp _http;
    private ISlackApiClient _sut;

    [SetUp]
    public void SetUp()
    {
        var jsonSettings = Default.JsonSettings();
        _http = new TestHttp();
        _sut = new SlackApiClient(_http, Default.UrlBuilder(jsonSettings), jsonSettings, "token");
    }

    [Test]
    public void Request_ErrorResponseWithErrorDetails_ThrowsWithDetails()
    {
        _http.Response = JObject.Parse("""
            {
                "ok": false,
                "error": "invalid_blocks",
                "errors": [{
                    "code": "failed_constraint",
                    "message": "unsupported type: invalid",
                    "pointer": "/0/text/type",
                    "constraint": { "type": "enum", "expected": ["plain_text", "mrkdwn"] }
                }]
            }
            """);

        _sut.Post("blocks.validate", new(), CancellationToken.None)
            .ShouldFailWith<SlackException>()
            .And(exception =>
                {
                    exception.Message.ShouldContain("/0/text/type: unsupported type: invalid");
                    exception.Message.ShouldNotContain("errors: ");
                    exception.ErrorDetails.ShouldBeSingular().And.Single().Constraint!.Value<string>("type").ShouldBe("enum");
                });
    }

    [Test]
    public void Request_ErrorResponseWithErrorsInOtherShape_ThrowsWithErrorsField()
    {
        _http.Response = JObject.Parse("""{ "ok": false, "error": "user_not_found", "errors": [{ "user": "U111111", "ok": false, "error": "user_not_found" }] }""");

        _sut.Post("conversations.invite", new(), CancellationToken.None)
            .ShouldFailWith<SlackException>()
            .And.ErrorFields["errors"].Single().Value<string>("user").ShouldBe("U111111");
    }

    [TestCase("\"explanation\"", "errors: explanation")]
    [TestCase("""["not an object"]""", """errors: ["not an object"]""")]
    [TestCase("""[{ "user": "U111111", "ok": false, "error": "user_not_found" }]""", """errors: [{"user":"U111111","ok":false,"error":"user_not_found"}]""")]
    public void Request_ErrorResponseWithErrorsInOtherShape_ThrowsWithErrorsLine(string errors, string expectedLine)
    {
        _http.Response = JObject.Parse($$"""{ "ok": false, "error": "invalid_manifest", "errors": {{errors}} }""");

        _sut.Post("apps.manifest.validate", new(), CancellationToken.None)
            .ShouldFailWith<SlackException>()
            .And(exception =>
                {
                    exception.ErrorCode.ShouldBe("invalid_manifest");
                    exception.ErrorDetails.ShouldBeEmpty();
                    exception.Message.ShouldContain(expectedLine);
                });
    }
}
