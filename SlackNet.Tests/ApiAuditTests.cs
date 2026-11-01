using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SlackNet.ApiAudit;
using SlackNet.Blocks;
using SlackNet.Events;

namespace SlackNet.Tests;

public class ApiAuditTests
{
    [Test]
    public async Task ApiInventory_Read_RecordsWireNamesAndCombinesOverloads()
    {
        var result = await ApiInventory.Read();

        result["users.info"].Parameters.ShouldMatch(["user", "include_locale"]);
        result["chat.unfurl"].Parameters.Contains("source").ShouldBe(true);
        result["chat.unfurl"].Parameters.Contains("unfurls").ShouldBe(true);
        result["chat.postMessage"].Parameters.Contains("metadata").ShouldBe(true);
        result["users.setPhoto"].Parameters.Contains("image").ShouldBe(true);
        result.ContainsKey("users.profile.get").ShouldBe(true);
        result.ContainsKey("chat.scheduledMessages.list").ShouldBe(true);
        result["conversations.open"].ResponseTypes.Count.ShouldBe(2);
        result["users.info"].ResponseType.GetProperty("User").ShouldNotBeNull();
    }

    [Test]
    public void ResponseAudit_Inspect_NestedAndExtensionDataPropertiesAreReported()
    {
        var sut = new ResponseAudit();

        sut.Inspect(JObject.Parse("""
            {"type":"message","future_event_field":true,"user":"U-private","blocks":[
              {"type":"section","text":{"type":"plain_text","text":"Private message"},"future_block_field":42}
            ]}
            """), typeof(Event), "event", "Live", "Dev workspace");
        var result = sut.Findings;

        result.Select(f => f.Path).ShouldMatch(["$.future_event_field", "$.blocks[].future_block_field"]);
        result.Single(f => f.Path == "$.blocks[].future_block_field").Model.ShouldBe(typeof(SectionBlock).FullName);
        string.Join(" ", sut.Shapes).ShouldNotContain("Private message");
        string.Join(" ", result).ShouldNotContain("U-private");
    }

    [Test]
    public void ResponseAudit_Inspect_UnknownEnumIsReportedWithoutItsValue()
    {
        var sut = new ResponseAudit();

        sut.Inspect(JObject.Parse("""{"type":"plain_text_input","action_id":"secret","focus_on_load":true,"dispatch_action_config":{"trigger_actions_on":["new_secret_trigger"]}}"""),
            typeof(BlockElement), "blocks", "Live", "Dev workspace");

        sut.Findings.Any(f => f.Kind == "Unknown enum value").ShouldBe(true);
        string.Join(" ", sut.Findings).ShouldNotContain("new_secret_trigger");
    }

    [Test]
    public void ResponseAudit_Inspect_DictionaryKeysAndMissingPropertiesAreNotFindings()
    {
        var sut = new ResponseAudit();

        sut.Inspect(JObject.Parse("""{"private_field_id":{"value":"private profile content"}}"""),
            typeof(Dictionary<string, UserProfileField>), "profile", "Live", "Dev workspace");

        sut.Findings.ShouldBeEmpty();
        sut.Shapes.Any(s => s.Path == "$.*.value").ShouldBe(true);
        string.Join(" ", sut.Shapes).ShouldNotContain("private_field_id");
    }

    [Test]
    public void ResponseAudit_Inspect_RepeatedArrayFieldsHaveStableFindingIds()
    {
        var sut = new ResponseAudit();

        sut.Inspect(JArray.Parse("""[{"new_field":1},{"new_field":2}]"""), typeof(List<User>), "users.list", "Live", "Dev workspace");
        var result = sut.Findings;

        result.Count.ShouldBe(1);
        result.Single().Id.ShouldBe(Finding.Create("Unmodeled property", "users.list", "$[].new_field", typeof(User).FullName, "Documentation", "another source").Id);
    }

    [Test]
    public void ResponseAudit_Inspect_IntentionalDynamicFieldsAndArrayQuirksAreCovered()
    {
        var sut = new ResponseAudit();

        sut.Inspect(JObject.Parse("""{"fields":[]}"""), typeof(UserProfile), "profile", "Live", "Dev workspace");
        sut.Inspect(JObject.Parse("""{"image_44":"private URL","image_default":true}"""), typeof(Icons), "team", "Live", "Dev workspace");

        sut.Findings.ShouldBeEmpty();
        string.Join(" ", sut.Shapes).ShouldNotContain("private URL");
    }

    [Test]
    public void SlackDocumentation_Parse_ArgumentsAndSuccessExamplesExcludeOtherSections()
    {
        var result = SlackDocumentation.Parse("""
            ## Arguments {#arguments}
            ### Required arguments
            **`token`**`string`Required
            **`channel`**`string`Required
            ### Optional arguments
            **`include_locale`**`boolean`Optional
            ## Usage info {#usage-info}
            **`not_an_argument`**
            ## Response {#response}
            ```
            {"ok":true,"channel":{"id":"C123"}}
            ```
            ```json
            {"ok":false,"error":"missing_scope"}
            ```
            ## Errors {#errors}
            **`another_non_argument`**
            """);

        result.Parameters.ShouldMatch(["channel", "include_locale"]);
        result.Examples.Count.ShouldBe(1);
    }

    [TestCase("# A redesigned docs page")]
    [TestCase("## Arguments\nA redesigned argument table")]
    public void SlackDocumentation_Parse_UnrecognizedFormatFailsExplicitly(string markdown) =>
        Assert.Throws<FormatException>(() => SlackDocumentation.Parse(markdown));

    [Test]
    public void SlackDocumentation_Parse_ElidedExampleDoesNotDiscardArguments()
    {
        var result = SlackDocumentation.Parse("""
            ## Arguments
            **`channel`**`string`Required
            ## Response
            ```
            {"ok":true,"icon":{...}}
            ```
            """);

        result.Parameters.ShouldMatch(["channel"]);
        result.UnparseableExamples.ShouldBe(1);
    }

    [Test]
    public async Task LiveProbes_Run_ScopeFailuresAndMissingFixtureRemainUnverified()
    {
        using var http = new HttpClient(new ProbeHandler());
        var findings = new List<Finding>();
        var observations = new List<Observation>();
        var sut = new LiveProbes(http, await ApiInventory.Read(), findings, observations);

        await sut.Run(null, null);

        findings.ShouldBeEmpty();
        observations.Count(o => o.Status == "Not covered").ShouldBe(5);
        observations.Any(o => o.Method == "conversations.history" && o.Detail == "No fixture channel supplied.").ShouldBe(true);
    }

    [Test]
    public async Task LiveProbes_Run_RateLimitIsReportedWithoutRetryingOrTreatingItAsSchemaDrift()
    {
        using var http = new HttpClient(new ProbeHandler(HttpStatusCode.TooManyRequests));
        var findings = new List<Finding>();
        var observations = new List<Observation>();
        var sut = new LiveProbes(http, await ApiInventory.Read(), findings, observations);

        await sut.Run("C-fixture", null);

        findings.ShouldBeEmpty();
        observations.Single(o => o.Method == "auth.test").Detail.ShouldContain("Rate limited");
        observations.Count.ShouldBe(2);
    }

    [Test]
    public async Task AuditRunner_Run_FullResponseExampleUsesAvailableFullResponseOverload()
    {
        using var http = new HttpClient(new SourceHandler("""[{"name":"conversations.open"}]""", """
            ## Arguments
            **`channel`**`string`Optional
            **`users`**`string`Optional
            **`return_im`**`boolean`Optional
            ## Response
            ```
            {"ok":true,"already_open":true,"no_op":false,"channel":{"id":"C123","is_im":true}}
            ```
            """));
        var output = Directory.CreateTempSubdirectory("SlackNet-ApiAuditTests-");
        try
        {
            var sut = new AuditRunner(http, output.FullName, false);

            var result = await sut.Run(null, null, null, null, []);

            result.Findings.ShouldBeEmpty();
            result.Observations.Single(o => o.Method == "conversations.open").Status.ShouldBe("Checked");
        }
        finally
        {
            output.Delete(true);
        }
    }

    [Test]
    public async Task AuditRunner_Run_BrokenIndexCannotEstablishMethodCoverage()
    {
        using var http = new HttpClient(new SourceHandler("[]", ""));
        var output = Directory.CreateTempSubdirectory("SlackNet-ApiAuditTests-");
        try
        {
            var sut = new AuditRunner(http, output.FullName, false);

            var result = await sut.Run(null, null, null, null, []);

            result.Observations.Single(o => o.Method == "Method index").Status.ShouldBe("Failed");
        }
        finally
        {
            output.Delete(true);
        }
    }

    private class SourceHandler(string index, string markdown) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(request.RequestUri.AbsolutePath.EndsWith("methods.json") ? index : markdown)
                });
    }

    private class ProbeHandler(HttpStatusCode status = HttpStatusCode.OK) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(status)
                {
                    Content = new StringContent(request.RequestUri.AbsolutePath.EndsWith("auth.test")
                        ? """{"ok":true,"user_id":"U-private"}"""
                        : """{"ok":false,"error":"missing_scope","needed":"private scope details"}""")
                });
    }
}
