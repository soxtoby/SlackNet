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
