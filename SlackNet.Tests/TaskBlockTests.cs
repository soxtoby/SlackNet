using EasyAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SlackNet.Blocks;

namespace SlackNet.Tests;

public class TaskBlockTests
{
    private JsonSerializerSettings _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).SerializerSettings;
    }

    [Test]
    public void PlanBlock_DeserializedFromSlackPayload_WithTasksThatHaveNoType()
    {
        var result = Deserialize<Block>(@"{
            ""type"": ""plan"",
            ""title"": ""Thinking completed"",
            ""tasks"": [
                {
                    ""task_id"": ""call_001"",
                    ""title"": ""Fetched user profile information"",
                    ""status"": ""in_progress"",
                    ""details"": { ""type"": ""rich_text"", ""block_id"": ""viMWO"", ""elements"": [ { ""type"": ""rich_text_section"", ""elements"": [ { ""type"": ""text"", ""text"": ""Searched database..."" } ] } ] }
                },
                { ""task_id"": ""call_002"", ""title"": ""Checked user permissions"", ""status"": ""pending"" },
                { ""task_id"": ""call_003"", ""title"": ""Generated report"", ""status"": ""complete"", ""sources"": [ { ""type"": ""url"", ""url"": ""https://example.com"", ""text"": ""Example.com"" } ] }
            ]
        }");

        var plan = result.ShouldBeA<PlanBlock>().And;
        plan.Title.ShouldBe("Thinking completed");
        plan.Tasks.Count.ShouldBe(3);
        plan.Tasks[0].TaskId.ShouldBe("call_001");
        plan.Tasks[0].Status.ShouldBe(TaskCardStatus.InProgress);
        plan.Tasks[0].Details.Elements.Count.ShouldBe(1);
        plan.Tasks[1].Status.ShouldBe(TaskCardStatus.Pending);
        plan.Tasks[2].Status.ShouldBe(TaskCardStatus.Complete);
        plan.Tasks[2].Sources[0].Url.ShouldBe("https://example.com");
        plan.Tasks[2].Sources[0].Text.ShouldBe("Example.com");
    }

    [Test]
    public void TaskCardBlock_DeserializedFromSlackPayload()
    {
        var result = Deserialize<Block>(@"{
            ""type"": ""task_card"",
            ""block_id"": ""b1"",
            ""task_id"": ""call_001"",
            ""title"": ""Fetched user profile information"",
            ""status"": ""error"",
            ""hide_title"": true,
            ""output"": { ""type"": ""rich_text"", ""elements"": [ { ""type"": ""rich_text_section"", ""elements"": [ { ""type"": ""text"", ""text"": ""Failed"" } ] } ] }
        }");

        var card = result.ShouldBeA<TaskCardBlock>().And;
        card.BlockId.ShouldBe("b1");
        card.TaskId.ShouldBe("call_001");
        card.Status.ShouldBe(TaskCardStatus.Error);
        card.HideTitle.ShouldBe(true);
        card.Output.Elements.Count.ShouldBe(1);
        card.Details.ShouldBeNull();
    }

    [Test]
    public void TaskCardBlock_SerializedToSlackPayload()
    {
        var result = Serialize(new TaskCardBlock
            {
                TaskId = "call_001",
                Title = "Fetched user profile information",
                Status = TaskCardStatus.InProgress,
                Sources = [new UrlSourceElement { Text = "Example.com", Url = "https://example.com" }]
            });

        JToken.DeepEquals(JObject.Parse(@"{ ""type"": ""task_card"", ""task_id"": ""call_001"", ""title"": ""Fetched user profile information"", ""status"": ""in_progress"", ""sources"": [ { ""type"": ""url"", ""url"": ""https://example.com"", ""text"": ""Example.com"" } ] }"), JObject.Parse(result))
            .ShouldBe(true, result);
    }

    [Test]
    public void TaskCardBlock_WithIcon_SerializedToSlackPayload()
    {
        var result = Serialize(new TaskCardBlock { TaskId = "call_001", Title = "Fetched user profile information", Status = TaskCardStatus.Complete, Icon = new TaskCardIcon { Name = "rocket" } });

        JToken.DeepEquals(JObject.Parse(@"{ ""type"": ""task_card"", ""task_id"": ""call_001"", ""title"": ""Fetched user profile information"", ""status"": ""complete"", ""icon"": { ""type"": ""icon"", ""name"": ""rocket"" } }"), JObject.Parse(result))
            .ShouldBe(true, result);
    }

    [Test]
    public void TaskCardBlock_WithIcon_DeserializedFromSlackPayload()
    {
        var result = Deserialize<Block>(@"{ ""type"": ""task_card"", ""task_id"": ""call_001"", ""title"": ""t"", ""status"": ""complete"", ""icon"": { ""type"": ""icon"", ""name"": ""rocket"" } }");

        result.ShouldBeA<TaskCardBlock>().And.Icon.Name.ShouldBe("rocket");
    }

    [Test]
    public void PlanBlock_SerializedToSlackPayload()
    {
        var result = Serialize(new PlanBlock
            {
                Title = "Thinking completed",
                Tasks = [new TaskCardBlock { TaskId = "call_002", Title = "Checked user permissions", Status = TaskCardStatus.Pending }]
            });

        JToken.DeepEquals(JObject.Parse(@"{ ""type"": ""plan"", ""title"": ""Thinking completed"", ""tasks"": [ { ""type"": ""task_card"", ""task_id"": ""call_002"", ""title"": ""Checked user permissions"", ""status"": ""pending"" } ] }"), JObject.Parse(result))
            .ShouldBe(true, result);
    }

    private string Serialize(object obj) => JsonConvert.SerializeObject(obj, _sut);

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}