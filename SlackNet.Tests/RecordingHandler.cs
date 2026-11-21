using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlackNet.Tests;

/// <summary>
/// Records the path of each request, and responds with <see cref="ResponseJson"/>.
/// </summary>
class RecordingHandler : HttpMessageHandler
{
    public IList<string> RequestPaths { get; } = [];
    public string ResponseJson { get; set; } = """{"ok":true}""";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestPaths.Add(request.RequestUri!.AbsolutePath);
        return Task.FromResult(new HttpResponseMessage { Content = new StringContent(ResponseJson, Encoding.UTF8, "application/json") });
    }
}
