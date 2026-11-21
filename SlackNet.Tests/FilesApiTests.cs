using System.Net.Http;
using System.Threading.Tasks;
using EasyAssertions;
using NUnit.Framework;
using SlackNet.WebApi;

namespace SlackNet.Tests;

public class FilesApiTests
{
    [Test]
    public async Task Upload_HttpClientProvided_UploadsContentWithProvidedClient()
    {
        var jsonSettings = Default.JsonSettings();
        var apiHandler = new RecordingHandler { ResponseJson = """{"ok":true,"upload_url":"https://files.slack.com/upload/v1/abc","file_id":"F123","files":[{"id":"F123"}]}""" };
        var uploadHandler = new RecordingHandler();
        var client = new SlackApiClient(() => new HttpClient(apiHandler), Default.UrlBuilder(jsonSettings), jsonSettings, "token");
        var sut = new FilesApi(client, () => new HttpClient(uploadHandler));

        await sut.Upload(new FileUpload("file.txt", "content"));

        uploadHandler.RequestPaths.ShouldMatch(["/upload/v1/abc"]);
    }
}
