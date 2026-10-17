using System.Net.Http;
using System.Threading;
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

    class RecordingHttp(SlackJsonSettings jsonSettings) : IHttp
    {
        public HttpRequestMessage Request { get; private set; }
        public JObject Body { get; private set; }
        public CancellationToken CancellationToken { get; private set; }

        public async Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
        {
            Request = requestMessage;
            CancellationToken = cancellationToken;
            Body = requestMessage.Content is null ? null : JObject.Parse(await requestMessage.Content.ReadAsStringAsync(cancellationToken));
            return JsonConvert.DeserializeObject<T>("""{"ok":true}""", jsonSettings.SerializerSettings);
        }
    }
}
