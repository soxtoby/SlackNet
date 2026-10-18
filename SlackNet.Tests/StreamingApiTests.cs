using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.WebApi;

namespace SlackNet.Tests;

public class StreamingApiTests
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
    public async Task StartStream_WithMarkdownText_SendsOnlyTheMarkdownArguments()
    {
        await _sut.Chat.StartStream("C123", "1717171717.123456", "Hello", recipientUserId: "U123", recipientTeamId: "T123");

        _http.Method.ShouldBe("chat.startStream");
        _http.Body.ShouldMatchJson(@"{ ""channel"": ""C123"", ""thread_ts"": ""1717171717.123456"", ""markdown_text"": ""Hello"", ""recipient_user_id"": ""U123"", ""recipient_team_id"": ""T123"" }");
    }

    [Test]
    public async Task StartStream_WithChunks_SendsChunksAndTaskDisplayMode()
    {
        await _sut.Chat.StartStream("C123", "1717171717.123456",
            chunks:
            [
                new MarkdownTextChunk { Text = "Let me help you with that!" },
                new TaskUpdateChunk { Id = "t1", Title = "Searching", Status = TaskCardStatus.InProgress }
            ],
            taskDisplayMode: TaskDisplayMode.Plan,
            iconEmoji: ":robot_face:",
            iconUrl: "https://example.com/icon.png",
            username: "Agent");

        _http.Body.ShouldMatchJson(@"{
            ""channel"": ""C123"",
            ""thread_ts"": ""1717171717.123456"",
            ""chunks"": [
                { ""type"": ""markdown_text"", ""text"": ""Let me help you with that!"" },
                { ""type"": ""task_update"", ""id"": ""t1"", ""title"": ""Searching"", ""status"": ""in_progress"" }
            ],
            ""task_display_mode"": ""plan"",
            ""icon_emoji"": "":robot_face:"",
            ""icon_url"": ""https://example.com/icon.png"",
            ""username"": ""Agent""
        }");
    }

    [Test]
    public async Task AppendStream_WithMarkdownText_SendsOnlyTheMarkdownArguments()
    {
        await _sut.Chat.AppendStream("C123", "1503435956.000247", "more");

        _http.Method.ShouldBe("chat.appendStream");
        _http.Body.ShouldMatchJson(@"{ ""channel"": ""C123"", ""ts"": ""1503435956.000247"", ""markdown_text"": ""more"" }");
    }

    [Test]
    public async Task AppendStream_WithChunks_SendsEveryKindOfChunk()
    {
        await _sut.Chat.AppendStream("C123", "1503435956.000247", chunks:
        [
            new MarkdownTextChunk { Text = "Here's what I found..." },
            new TaskUpdateChunk
                {
                    Id = "t1",
                    Title = "Remind Sandra how amazing she is",
                    HideTitle = false,
                    Status = TaskCardStatus.Complete,
                    Details = "wow such good details",
                    Output = "amazing output here",
                    Sources = [new UrlSource { Text = "Example.com", Url = "https://example.com" }]
                },
            new PlanUpdateChunk { Title = "Sandra's new and improved plan" },
            new BlocksChunk { Blocks = [new MarkdownBlock { Text = "**Sandra's plan outline**" }] }
        ]);

        _http.Body.ShouldMatchJson(@"{
            ""channel"": ""C123"",
            ""ts"": ""1503435956.000247"",
            ""chunks"": [
                { ""type"": ""markdown_text"", ""text"": ""Here's what I found..."" },
                {
                    ""type"": ""task_update"",
                    ""id"": ""t1"",
                    ""title"": ""Remind Sandra how amazing she is"",
                    ""hide_title"": false,
                    ""status"": ""complete"",
                    ""details"": ""wow such good details"",
                    ""output"": ""amazing output here"",
                    ""sources"": [ { ""type"": ""url"", ""url"": ""https://example.com"", ""text"": ""Example.com"" } ]
                },
                { ""type"": ""plan_update"", ""title"": ""Sandra's new and improved plan"" },
                { ""type"": ""blocks"", ""blocks"": [ { ""type"": ""markdown"", ""text"": ""**Sandra's plan outline**"" } ] }
            ]
        }");
    }

    [Test]
    public async Task TaskUpdateChunk_WithIcon_SendsIcon()
    {
        await _sut.Chat.AppendStream("C123", "1503435956.000247", chunks: [new TaskUpdateChunk { Id = "t1", Title = "Task", Status = TaskCardStatus.InProgress, Icon = new TaskCardIcon { Name = "bug" } }]);

        _http.Body.ShouldMatchJson(@"{ ""channel"": ""C123"", ""ts"": ""1503435956.000247"", ""chunks"": [ { ""type"": ""task_update"", ""id"": ""t1"", ""title"": ""Task"", ""status"": ""in_progress"", ""icon"": { ""type"": ""icon"", ""name"": ""bug"" } } ] }");
    }

    [TestCase(TaskCardStatus.Pending, "pending")]
    [TestCase(TaskCardStatus.InProgress, "in_progress")]
    [TestCase(TaskCardStatus.Complete, "complete")]
    [TestCase(TaskCardStatus.Error, "error")]
    public async Task TaskUpdateChunk_SendsStatusInSlackCase(TaskCardStatus status, string expected)
    {
        await _sut.Chat.AppendStream("C123", "1503435956.000247", chunks: [new TaskUpdateChunk { Id = "t1", Title = "Task", Status = status }]);

        ((string)_http.Body["chunks"][0]["status"]).ShouldBe(expected);
    }

    [Test]
    public async Task StopStream_WithoutChunksOrSessionStatus_SendsOnlyTheMarkdownAndBlocks()
    {
        await _sut.Chat.StopStream("C123", "1503435956.000247", "done", blocks: [new MarkdownBlock { Text = "footer" }]);

        _http.Method.ShouldBe("chat.stopStream");
        _http.Body.ShouldMatchJson(@"{ ""channel"": ""C123"", ""ts"": ""1503435956.000247"", ""markdown_text"": ""done"", ""blocks"": [ { ""type"": ""markdown"", ""text"": ""footer"" } ] }");
    }

    [Test]
    public async Task StopStream_WithChunksAndSessionStatus_SendsThem()
    {
        await _sut.Chat.StopStream("C123", "1503435956.000247",
            chunks: [new MarkdownTextChunk { Text = "Hope this helps!" }],
            sessionStatus: AgentSessionStatus.Suspended);

        _http.Body.ShouldMatchJson(@"{ ""channel"": ""C123"", ""ts"": ""1503435956.000247"", ""chunks"": [ { ""type"": ""markdown_text"", ""text"": ""Hope this helps!"" } ], ""session_status"": ""suspended"" }");
    }

    [Test]
    public void AppendStream_StoppedByUser_ThrowsSlackExceptionWithErrorCode()
    {
        _http.Response = JObject.Parse(@"{ ""ok"": false, ""error"": ""stopped_by_user"" }");

        var exception = Assert.ThrowsAsync<SlackException>(() => _sut.Chat.AppendStream("C123", "1503435956.000247", "more"));

        exception.ErrorCode.ShouldBe("stopped_by_user");
    }
}
