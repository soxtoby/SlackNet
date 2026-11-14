using System;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EasyAssertions;
using NUnit.Framework;

namespace SlackNet.Tests;

public class AdminAnalyticsApiTests
{
    private StubHandler _handler;
    private ISlackApiClient _sut;

    [SetUp]
    public void SetUp()
    {
        var jsonSettings = Default.JsonSettings();
        _handler = new StubHandler();
        _sut = new SlackApiClient(Default.Http(jsonSettings, () => new HttpClient(_handler)), Default.UrlBuilder(jsonSettings), jsonSettings, "token");
    }

    [TearDown]
    public void TearDown() => _handler.Dispose();

    [Test]
    public void GetMemberAnalytics_GzippedFile_ReadsEachRecord()
    {
        _handler.Content = GzipContent(
            """
            {"enterprise_id":"E1","date":"2020-10-05","user_id":"W1","is_active":true,"messages_posted_count":123}
            {"enterprise_id":"E1","date":"2020-10-05","user_id":"W2","is_active":false,"messages_posted_count":0}

            """);

        _sut.AdminAnalytics.GetMemberAnalytics(new DateTime(2020, 10, 5))
            .ShouldComplete()
            .And.ItemsSatisfy(
                first =>
                    {
                        first.UserId.ShouldBe("W1");
                        first.Date.ShouldBe(new DateTime(2020, 10, 5));
                        first.MessagesPostedCount.ShouldBe(123);
                    },
                second => second.UserId.ShouldBe("W2"));
        var query = HttpUtility.ParseQueryString(_handler.Request.RequestUri!.Query);
        query["type"].ShouldBe("member");
        query["date"].ShouldBe("2020-10-05");
    }

    [Test]
    public async Task GetPublicChannelMetadata_FileStillDownloading_YieldsRecordsAlreadyReceived()
    {
        using var downloadingFile = new AnonymousPipeServerStream(PipeDirection.Out);
        using var receivedFile = new AnonymousPipeClientStream(PipeDirection.In, downloadingFile.ClientSafePipeHandle);
        _handler.Content = new TrackedContent(() => receivedFile, "application/gzip");
        var gzip = new GZipStream(downloadingFile, CompressionMode.Compress, leaveOpen: true); // Never completed, so the file is still downloading
        await gzip.WriteAsync(Encoding.UTF8.GetBytes("""{"channel_id":"C1","name":"general"}""" + "\n"));
        await gzip.FlushAsync();

        await using var records = _sut.AdminAnalytics.GetPublicChannelMetadata().GetAsyncEnumerator();
        var hasRecord = await records.MoveNextAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5));

        hasRecord.ShouldBe(true);
        records.Current.Name.ShouldBe("general");
    }

    [Test]
    public void GetPublicChannelAnalytics_ErrorResponse_ThrowsSlackException()
    {
        _handler.Content = new StringContent("""{"ok":false,"error":"file_not_yet_available"}""", Encoding.UTF8, "application/json");

        _sut.AdminAnalytics.GetPublicChannelAnalytics(new DateTime(2020, 11, 14))
            .ShouldFail()
            .And.ShouldBeA<SlackException>()
            .And.ErrorCode.ShouldBe("file_not_yet_available");
    }

    [Test]
    public void GetMemberAnalytics_EnumeratedToEnd_DisposesResponseStream()
    {
        var content = GzipContent("""{"user_id":"W1"}""");
        _handler.Content = content;

        _sut.AdminAnalytics.GetMemberAnalytics(new DateTime(2020, 10, 5)).ShouldComplete();

        content.IsReadStreamDisposed.ShouldBe(true);
    }

    [Test]
    public async Task GetMemberAnalytics_EnumerationStoppedEarly_DisposesResponseStream()
    {
        var content = GzipContent(
            """
            {"user_id":"W1"}
            {"user_id":"W2"}
            """);
        _handler.Content = content;

        await using (var records = _sut.AdminAnalytics.GetMemberAnalytics(new DateTime(2020, 10, 5)).GetAsyncEnumerator())
            await records.MoveNextAsync();

        content.IsReadStreamDisposed.ShouldBe(true);
    }

    [Test]
    public void GetMemberAnalytics_CancelledWhileErrorResponseStalls_StopsAndDisposesResponse()
    {
        using var stalledResponse = new AnonymousPipeServerStream(PipeDirection.Out);
        using var receivedResponse = new AnonymousPipeClientStream(PipeDirection.In, stalledResponse.ClientSafePipeHandle);
        var content = new TrackedContent(() => receivedResponse, "application/json");
        _handler.Content = content;
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        _sut.AdminAnalytics.GetMemberAnalytics(new DateTime(2020, 10, 5), cancellation.Token)
            .ShouldFail()
            .And.ShouldBeA<OperationCanceledException>();
        content.IsDisposed.ShouldBe(true);
    }

    private static TrackedContent GzipContent(string text)
    {
        using var compressed = new MemoryStream();
        using (var gzip = new GZipStream(compressed, CompressionMode.Compress))
            gzip.Write(Encoding.UTF8.GetBytes(text));
        var bytes = compressed.ToArray();
        return new TrackedContent(() => new MemoryStream(bytes), "application/gzip");
    }

    class StubHandler : HttpMessageHandler
    {
        public HttpContent Content { get; set; }
        public HttpRequestMessage Request { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(new HttpResponseMessage { Content = Content });
        }
    }

    /// <summary>
    /// Streams content like a real response, and records whether it, or the stream it was read from, has been disposed.
    /// </summary>
    class TrackedContent : HttpContent
    {
        private readonly Func<Stream> _openStream;

        public TrackedContent(Func<Stream> openStream, string mediaType)
        {
            _openStream = openStream;
            Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        }

        public bool IsDisposed { get; private set; }
        public bool IsReadStreamDisposed => _readStream is { CanRead: false };

        private Stream _readStream;

        protected override Task<Stream> CreateContentReadStreamAsync() => Task.FromResult(_readStream = _openStream());

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) => _openStream().CopyToAsync(stream);

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context, CancellationToken cancellationToken) =>
            _openStream().CopyToAsync(stream, cancellationToken);

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}
