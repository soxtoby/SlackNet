using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SlackNet.Tests.Objects;

public class FileTests
{
    private JsonSerializerSettings _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).SerializerSettings;
    }

    [Test]
    public void File_FromSharedChannel_HasUserTeamAndSourceTeam()
    {
        var result = Deserialize<File>(@"{ ""id"": ""F12345678"", ""user"": ""U12345678"", ""user_team"": ""T111"", ""source_team"": ""T222"" }");

        result.UserTeam.ShouldBe("T111");
        result.SourceTeam.ShouldBe("T222");
    }

    [Test]
    public void File_WithShareAndMediaFields_HasThem()
    {
        var result = Deserialize<File>(@"{ ""id"": ""F12345678"", ""media_display_type"": ""unknown"", ""display_as_bot"": true, ""has_more_shares"": true }");

        result.MediaDisplayType.ShouldBe("unknown");
        result.DisplayAsBot.ShouldBe(true);
        result.HasMoreShares.ShouldBe(true);
    }

    [Test]
    public void File_WithoutThoseFields_HasDefaults()
    {
        var result = Deserialize<File>(@"{ ""id"": ""F12345678"" }");

        result.UserTeam.ShouldBeNull();
        result.SourceTeam.ShouldBeNull();
        result.MediaDisplayType.ShouldBeNull();
        result.DisplayAsBot.ShouldBe(false);
        result.HasMoreShares.ShouldBe(false);
    }

    private T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _sut);
}
