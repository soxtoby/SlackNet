#nullable disable
#if NETSTANDARD2_0
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SlackNet;

/// <summary>
/// HttpContent reads can't be cancelled in .NET Standard 2.0, so these only stop waiting for the read when cancelled.
/// Dispose the response to stop the read itself.
/// </summary>
static class HttpContentExtensions
{
    extension(HttpContent content)
    {
        public Task<string> ReadAsStringAsync(CancellationToken cancellationToken) =>
            content.ReadAsStringAsync().WaitAsync(cancellationToken);

        public Task<Stream> ReadAsStreamAsync(CancellationToken cancellationToken) =>
            content.ReadAsStreamAsync().WaitAsync(cancellationToken);
    }
}
#endif
