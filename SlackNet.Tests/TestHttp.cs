#nullable enable
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json.Linq;
using SlackNet.WebApi;

namespace SlackNet.Tests;

class TestHttp : IHttp
{
    public string Method { get; private set; } = "";
    public JObject Body { get; private set; } = new();
    public JObject Response { get; set; } = new() { ["ok"] = true };

    public async Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        Method = requestMessage.RequestUri!.Segments[^1];
        Body = JObject.Parse(await requestMessage.Content!.ReadAsStringAsync().ConfigureAwait(false));
        return (T)(object)new WebApiResponse { Ok = (bool)Response["ok"]!, Data = Response };
    }
}

static class JsonAssertions
{
    public static void ShouldMatchJson(this JObject actual, string expectedJson) =>
        JToken.DeepEquals(JObject.Parse(expectedJson), actual)
            .ShouldBe(true, $"Expected JSON {JObject.Parse(expectedJson)} but was {actual}");
}