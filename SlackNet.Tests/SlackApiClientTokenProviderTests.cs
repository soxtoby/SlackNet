using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using SlackNet.WebApi;

namespace SlackNet.Tests;

public class SlackApiClientTokenProviderTests
{
    private IHttp _http;
    private SlackJsonSettings _jsonSettings;
    private ISlackApiTokenProvider _apiTokenProvider;
    private ISlackAppLevelTokenProvider _appLevelTokenProvider;

    [SetUp]
    public void Setup()
    {
        _http = Substitute.For<IHttp>();
        _jsonSettings = Default.JsonSettings();
        _apiTokenProvider = Substitute.For<ISlackApiTokenProvider>();
        _appLevelTokenProvider = Substitute.For<ISlackAppLevelTokenProvider>();

        _http.Execute<WebApiResponse>(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(new WebApiResponse { Ok = true, Data = new JObject { ["url"] = "wss://socket" } });
    }

    [Test]
    public async Task ApiRequest_UsesApiTokenProvider()
    {
        SlackApiCallContext context = null;
        _apiTokenProvider.GetApiToken(Arg.Do<SlackApiCallContext>(c => context = c), Arg.Any<CancellationToken>())
            .Returns("api-token");

        await Client().Get<object>("api.test", new Dictionary<string, object> { ["foo"] = "bar" }, CancellationToken.None);

        context.ApiMethod.ShouldBe("api.test");
        context.Arguments["foo"].ShouldBe("bar");
        context.OriginatingRequest.ShouldBeNull();
        _ = _appLevelTokenProvider.DidNotReceive().GetAppLevelToken(Arg.Any<SlackApiCallContext>(), Arg.Any<CancellationToken>());
        await _http.Received().Execute<WebApiResponse>(
            Arg.Is<HttpRequestMessage>(r => r.Headers.Authorization.ToString() == "Bearer api-token"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task AppsConnectionsOpen_UsesAppLevelTokenProvider()
    {
        SlackApiCallContext context = null;
        _appLevelTokenProvider.GetAppLevelToken(Arg.Do<SlackApiCallContext>(c => context = c), Arg.Any<CancellationToken>())
            .Returns("app-token");

        await Client().AppsConnections.Open();

        context.ApiMethod.ShouldBe("apps.connections.open");
        _ = _apiTokenProvider.DidNotReceive().GetApiToken(Arg.Any<SlackApiCallContext>(), Arg.Any<CancellationToken>());
        await _http.Received().Execute<WebApiResponse>(
            Arg.Is<HttpRequestMessage>(r => r.Headers.Authorization.ToString() == "Bearer app-token"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task WithAccessToken_OverridesBothTokenProviders()
    {
        var sut = Client().WithAccessToken("override");

        await sut.Get<object>("api.test", new(), CancellationToken.None);
        await sut.AppsConnections.Open();
        await sut.AppsEventAuthorizations.List("event-context");

        _ = _apiTokenProvider.DidNotReceive().GetApiToken(Arg.Any<SlackApiCallContext>(), Arg.Any<CancellationToken>());
        _ = _appLevelTokenProvider.DidNotReceive().GetAppLevelToken(Arg.Any<SlackApiCallContext>(), Arg.Any<CancellationToken>());
        await _http.Received(3).Execute<WebApiResponse>(
            Arg.Is<HttpRequestMessage>(r => r.Headers.Authorization.ToString() == "Bearer override"),
            Arg.Any<CancellationToken>());
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task AppsEventAuthorizationsList_UsesAppLevelTokenProvider(bool useRawApiCall)
    {
        _apiTokenProvider.GetApiToken(Arg.Any<SlackApiCallContext>(), Arg.Any<CancellationToken>())
            .Returns("api-token");
        _appLevelTokenProvider.GetAppLevelToken(Arg.Any<SlackApiCallContext>(), Arg.Any<CancellationToken>())
            .Returns("app-token");
        var sut = Client();

        if (useRawApiCall)
            await sut.Post<object>("apps.event.authorizations.list", new() { ["event_context"] = "event-context" }, CancellationToken.None);
        else
            await sut.AppsEventAuthorizations.List("event-context");

        await _http.Received().Execute<WebApiResponse>(
            Arg.Is<HttpRequestMessage>(r => r.Headers.Authorization.ToString() == "Bearer app-token"),
            Arg.Any<CancellationToken>());
        _ = _apiTokenProvider.DidNotReceive().GetApiToken(Arg.Any<SlackApiCallContext>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ApiRequest_IncludesOriginatingSlackRequest()
    {
        var originatingRequest = new SlackRequestContext();
        SlackApiCallContext context = null;
        _apiTokenProvider.GetApiToken(Arg.Do<SlackApiCallContext>(c => context = c), Arg.Any<CancellationToken>())
            .Returns("api-token");
        SlackRequestContext.Current = originatingRequest;

        try
        {
            await Client().Get<object>("api.test", new Dictionary<string, object>(), CancellationToken.None);
        }
        finally
        {
            SlackRequestContext.Current = null;
        }

        context.OriginatingRequest.ShouldReferTo(originatingRequest);
    }

    private SlackApiClient Client() =>
        new(_http, Default.UrlBuilder(_jsonSettings), _jsonSettings, _apiTokenProvider, _appLevelTokenProvider);
}
