using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace SlackNet.Tests;

public class WebApiRequestTests
{
    private ISlackApiClient _sut;
    private RecordingHttp _http;

    [SetUp]
    public void SetUp()
    {
        var jsonSettings = Default.JsonSettings();
        _http = new RecordingHttp(jsonSettings);
        _sut = new SlackApiClient(_http, Default.UrlBuilder(jsonSettings), jsonSettings, "token");
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task ViewsApi_InteractionCredential_SendsSelectedCredential(bool push, bool interactivity)
    {
        var view = new ModalViewDefinition { CallbackId = "review_request" };
        using var cancellation = new CancellationTokenSource();

        await ((push, interactivity) switch
            {
                (false, false) => _sut.Views.Open(view, triggerId: "trigger", cancellationToken: cancellation.Token),
                (false, true) => _sut.Views.Open(view, interactivityPointer: "pointer", cancellationToken: cancellation.Token),
                (true, false) => _sut.Views.Push(view, triggerId: "trigger", cancellationToken: cancellation.Token),
                (true, true) => _sut.Views.Push(view, interactivityPointer: "pointer", cancellationToken: cancellation.Token)
            });

        _http.Request.Method.ShouldBe(HttpMethod.Post);
        _http.Request.RequestUri.AbsolutePath.ShouldBe(push ? "/api/views.push" : "/api/views.open");
        _http.Body.Value<string>(interactivity ? "interactivity_pointer" : "trigger_id").ShouldBe(interactivity ? "pointer" : "trigger");
        _http.Body.ContainsKey(interactivity ? "trigger_id" : "interactivity_pointer").ShouldBe(false);
        _http.Body["view"]["callback_id"].Value<string>().ShouldBe("review_request");
        _http.CancellationToken.ShouldBe(cancellation.Token);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task ViewsApi_TriggerFirstOverload_PreservesRequest(bool push)
    {
        var view = new ModalViewDefinition { CallbackId = "review_request" };
        using var cancellation = new CancellationTokenSource();

#pragma warning disable CS0618 // Exercises the obsolete compatibility overloads.
        await (push
            ? _sut.Views.Push("trigger", view, cancellation.Token)
            : _sut.Views.Open("trigger", view, cancellation.Token));
#pragma warning restore CS0618

        _http.Request.RequestUri.AbsolutePath.ShouldBe(push ? "/api/views.push" : "/api/views.open");
        _http.Body.Value<string>("trigger_id").ShouldBe("trigger");
        _http.Body.ContainsKey("interactivity_pointer").ShouldBe(false);
        _http.Body["view"]["callback_id"].Value<string>().ShouldBe("review_request");
        _http.CancellationToken.ShouldBe(cancellation.Token);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task ViewsApi_WithoutCredentials_OmitsOptionalCredentials(bool push)
    {
        var view = new ModalViewDefinition { CallbackId = "review_request" };

        await (push ? _sut.Views.Push(view) : _sut.Views.Open(view));

        _http.Body.ContainsKey("trigger_id").ShouldBe(false);
        _http.Body.ContainsKey("interactivity_pointer").ShouldBe(false);
        _http.Body["view"]["callback_id"].Value<string>().ShouldBe("review_request");
    }

    [Test]
    public async Task WorkflowsFeaturedApi_ListWithChannelIds_PostsArray()
    {
        await _sut.WorkflowsFeatured.List(["C123", "C456"]);

        _http.Request.Method.ShouldBe(HttpMethod.Post);
        _http.Request.RequestUri.AbsolutePath.ShouldBe("/api/workflows.featured.list");
        _http.Request.RequestUri.Query.ShouldBe(string.Empty);
        _http.Body["channel_ids"].Values<string>().ShouldMatch(["C123", "C456"]);
    }

    [Test]
    public async Task WorkflowTriggerPermissionsApi_ListWithTriggerId_PostsBody()
    {
        await _sut.WorkflowTriggerPermissions.List("Ft123");

        _http.Request.Method.ShouldBe(HttpMethod.Post);
        _http.Request.RequestUri.AbsolutePath.ShouldBe("/api/workflows.triggers.permissions.list");
        _http.Request.RequestUri.Query.ShouldBe(string.Empty);
        _http.Body.Value<string>("trigger_id").ShouldBe("Ft123");
    }

    [Test]
    public async Task SlackbotSkillsApi_UpdateWithSkillMcpServers_PostsOnlyServerReferences()
    {
        SkillMcpServer[] mcpServers =
            [
                new SkillMcpServer { ServiceType = "app", AppId = "A123", ServerId = "Amcp456", DisplayName = "Google Calendar", IsAvailable = true },
                new SkillMcpServer { ServiceType = "github" }
            ];
        _http.ResponseJson = """{"ok":true,"skill":{}}""";

        await _sut.SlackbotSkills.Update("F123", mcpServers: mcpServers);

        _http.Request.RequestUri.AbsolutePath.ShouldBe("/api/slackbot.skills.update");
        JToken.DeepEquals(_http.Body["mcp_servers"], JArray.Parse("""[{ "service_type": "app", "app_id": "A123", "server_id": "Amcp456" }, { "service_type": "github" }]"""))
            .ShouldBe(true);
    }

    [Test]
    public async Task AgentsConversationsApi_SetPropertiesWithDraftPullRequest_PostsOnlyEmptyPullRequestStatus()
    {
        await _sut.AgentsConversations.SetProperties("C123", new CodeChannelProperties { PrStatus = PullRequestStatus.Draft });

        _http.Request.RequestUri.AbsolutePath.ShouldBe("/api/agents.conversations.setProperties");
        JToken.DeepEquals(_http.Body["code_channel"], JObject.Parse("""{ "pr_status": "" }"""))
            .ShouldBe(true);
    }

    [Test]
    public async Task AgentsConversationsApi_SetHtmlViewWithCspResourceDomains_PostsThemInCsp()
    {
        _http.ResponseJson = """{"ok":true,"view_id":"Ct123","type":"html"}""";

        await _sut.AgentsConversations.SetHtmlView("C123", "reports/coverage.html", "<html></html>", cspResourceDomains: ["https://cdn.jsdelivr.net"]);

        _http.Request.RequestUri.AbsolutePath.ShouldBe("/api/agents.conversations.setView");
        _http.Body.Value<string>("type").ShouldBe("html");
        JToken.DeepEquals(_http.Body["csp"], JObject.Parse("""{ "resource_domains": ["https://cdn.jsdelivr.net"] }"""))
            .ShouldBe(true);
    }

    [Test]
    public async Task AdminAppsActivitiesApi_ListWithDateRange_SendsEpochMicroseconds()
    {
        var minDateCreated = DateTimeOffset.FromUnixTimeMilliseconds(1646665572336).AddTicks(2510).UtcDateTime;
        var maxDateCreated = DateTimeOffset.FromUnixTimeMilliseconds(1646665572336).AddTicks(2990).UtcDateTime;

        await _sut.AdminAppsActivities.List(minDateCreated: minDateCreated, maxDateCreated: maxDateCreated);

        var query = HttpUtility.ParseQueryString(_http.Request.RequestUri.Query);
        query["min_date_created"].ShouldBe("1646665572336251");
        query["max_date_created"].ShouldBe("1646665572336299");
    }

    [Test]
    public async Task AdminAppsActivitiesApi_ListActivity_ReadsCreatedDateFromEpochMicroseconds()
    {
        _http.ResponseJson = """{"ok":true,"activities":[{"level":"info","event_type":"function_execution_started","source":"slack","component_type":"functions","payload":{},"created":1650463798824317}]}""";

        var result = await _sut.AdminAppsActivities.List();

        result.Activities.ShouldBeSingular()
            .And.Single().CreatedDate.ShouldBe(DateTimeOffset.FromUnixTimeMilliseconds(1650463798824).AddTicks(3170).UtcDateTime);
    }

    [Test]
    public async Task AdminConversationsApi_SetConversationPrefs_SendsPrefsInSlacksFormat()
    {
        await _sut.AdminConversations.SetConversationPrefs("C123",
            whoCanPost: new ConversationPermission
                {
                    Types = [ConversationPermissionType.Admin, ConversationPermissionType.OrgAdmin],
                    UserIds = ["U123", "U456"],
                    UsergroupIds = ["S123"]
                },
            canThread: new ConversationPermission { Types = [ConversationPermissionType.Ra] },
            enableAtChannel: false,
            enableAtHere: false);

        var query = HttpUtility.ParseQueryString(_http.Request.RequestUri!.Query);
        query["prefs"].ShouldBe("""
            {"who_can_post":"type:admin,type:org_admin,user:U123,user:U456,subteam:S123","can_thread":"type:ra","enable_at_channel":"false","enable_at_here":"false"}
            """);
    }

    class RecordingHttp(SlackJsonSettings jsonSettings) : IHttp
    {
        public HttpRequestMessage Request { get; private set; }
        public JObject Body { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
        public string ResponseJson { get; set; } = """{"ok":true}""";

        public async Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
        {
            Request = requestMessage;
            CancellationToken = cancellationToken;
            Body = requestMessage.Content is null ? null : JObject.Parse(await requestMessage.Content.ReadAsStringAsync(cancellationToken));
            return JsonConvert.DeserializeObject<T>(ResponseJson, jsonSettings.SerializerSettings);
        }

        public Task<HttpResponseMessage> ExecuteStreaming(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
