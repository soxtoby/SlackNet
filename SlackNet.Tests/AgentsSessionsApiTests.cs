using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SlackNet.WebApi;

namespace SlackNet.Tests;

public class AgentsSessionsApiTests
{
    private TestHttp _http;
    private ISlackApiClient _sut;

    [SetUp]
    public void SetUp()
    {
        var jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
        _http = new TestHttp();
        _sut = new SlackApiClient(_http, Default.UrlBuilder(jsonSettings), jsonSettings, "token");
    }

    [Test]
    public async Task SetStatus_SendsSessionArguments()
    {
        await _sut.AgentsSessions.SetStatus("C123", "1717171717.123456", AgentSessionStatus.Processing,
            title: "Scuba diving research", initiatorUserId: "U123", iconEmoji: ":robot_face:", username: "Custom Agent Name");

        _http.Method.ShouldBe("agents.sessions.setStatus");
        _http.Body.ShouldMatchJson(@"{ ""channel_id"": ""C123"", ""thread_ts"": ""1717171717.123456"", ""status"": ""processing"", ""title"": ""Scuba diving research"", ""initiator_user_id"": ""U123"", ""icon_emoji"": "":robot_face:"", ""username"": ""Custom Agent Name"" }");
    }

    [TestCase(AgentSessionStatus.Active, "active")]
    [TestCase(AgentSessionStatus.Processing, "processing")]
    [TestCase(AgentSessionStatus.Suspended, "suspended")]
    [TestCase(AgentSessionStatus.Closed, "closed")]
    public async Task SetStatus_SendsStatusInSlackCase(AgentSessionStatus status, string expected)
    {
        await _sut.AgentsSessions.SetStatus("C123", "1717171717.123456", status);

        _http.Body.ShouldMatchJson(@"{ ""channel_id"": ""C123"", ""thread_ts"": ""1717171717.123456"", ""status"": """ + expected + @""" }");
    }

    [Test]
    public async Task SetStatus_InSessionChannel_OmitsThreadTs()
    {
        await _sut.AgentsSessions.SetStatus("C123", null, AgentSessionStatus.Active);

        _http.Body.ShouldMatchJson(@"{ ""channel_id"": ""C123"", ""status"": ""active"" }");
    }

    [Test]
    public async Task SetStatus_ReadsSessionStatusAndAgentStatus()
    {
        _http.Response = JObject.Parse(@"{ ""ok"": true, ""status"": ""processing"", ""agent_status"": ""active"", ""title"": ""Scuba diving research"" }");

        var result = await _sut.AgentsSessions.SetStatus("C123", "1717171717.123456", AgentSessionStatus.Active);

        result.Status.ShouldBe(AgentSessionStatus.Processing);
        result.AgentStatus.ShouldBe(AgentSessionStatus.Active);
        result.Title.ShouldBe("Scuba diving research");
    }
}