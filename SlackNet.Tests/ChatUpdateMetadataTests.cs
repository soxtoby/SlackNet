#nullable enable
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SlackNet.WebApi;

namespace SlackNet.Tests;

public class ChatUpdateMetadataTests
{
    private RecordingHttp _http = null!;
    private ISlackApiClient _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
        _http = new RecordingHttp();
        _sut = new SlackApiClient(_http, Default.UrlBuilder(jsonSettings), jsonSettings, "token");
    }

    [Test]
    public async Task Update_WithoutMetadata_SendsNoMetadata()
    {
        await _sut.Chat.Update(new MessageUpdate { ChannelId = "C123", Ts = "1503435956.000247", Text = "edited" });

        _http.Method.ShouldBe("chat.update");
        _http.Body.ContainsKey("metadata").ShouldBe(false);
    }

    [Test]
    public async Task Update_WithMetadataJson_SendsMetadata()
    {
        await _sut.Chat.Update(new MessageUpdate
            {
                ChannelId = "C123",
                Ts = "1503435956.000247",
                Text = "edited",
                MetadataJson = new MessageMetadata { EventType = "task_created", EventPayload = JObject.Parse(@"{ ""id"": ""TK-42"" }") }
            });

        _http.Body["channel"]!.Value<string>().ShouldBe("C123");
        _http.Body["text"]!.Value<string>().ShouldBe("edited");
        JToken.DeepEquals(JObject.Parse(@"{ ""event_type"": ""task_created"", ""event_payload"": { ""id"": ""TK-42"" } }"), _http.Body["metadata"]).ShouldBe(true);
    }

    [Test]
    public async Task Update_WithMetadataObject_SendsItAsMetadata()
    {
        await _sut.Chat.Update(new MessageUpdate { ChannelId = "C123", Ts = "1503435956.000247", MetadataObject = new TaskCreated { TaskId = "TK-42" } });

        JToken.DeepEquals(JObject.Parse(@"{ ""event_type"": ""task_created"", ""event_payload"": { ""task_id"": ""TK-42"" } }"), _http.Body["metadata"]).ShouldBe(true);
    }

    [Test]
    public async Task Update_WithMetadataJsonAndMetadataObject_SendsMetadataJson()
    {
        await _sut.Chat.Update(new MessageUpdate
            {
                ChannelId = "C123",
                Ts = "1503435956.000247",
                MetadataJson = new MessageMetadata { EventType = "from_json", EventPayload = new JObject() },
                MetadataObject = new TaskCreated { TaskId = "TK-42" }
            });

        _http.Body["metadata"]!["event_type"]!.Value<string>().ShouldBe("from_json");
    }

    [SlackType("task_created")]
    private class TaskCreated
    {
        public string TaskId { get; set; } = "";
    }

    private class RecordingHttp : IHttp
    {
        public string Method { get; private set; } = "";
        public JObject Body { get; private set; } = new();

        public async Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
        {
            Method = requestMessage.RequestUri!.Segments[^1];
            Body = JObject.Parse(await requestMessage.Content!.ReadAsStringAsync().ConfigureAwait(false));
            return (T)(object)new WebApiResponse { Ok = true, Data = new JObject { ["ok"] = true } };
        }
    }
}
