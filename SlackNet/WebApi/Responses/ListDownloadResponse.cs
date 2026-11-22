#nullable disable
using Newtonsoft.Json;

namespace SlackNet.WebApi;

public class ListDownloadResponse
{
    public string Status { get; set; }
    [JsonProperty("download_url")]
    public string Url { get; set; }
}