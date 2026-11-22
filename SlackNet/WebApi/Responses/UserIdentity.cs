#nullable disable
using Newtonsoft.Json;

namespace SlackNet.WebApi;

public class UserIdentity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    [JsonProperty("image_24")]
    public string Image24 { get; set; }
    [JsonProperty("image_32")]
    public string Image32 { get; set; }
    [JsonProperty("image_48")]
    public string Image48 { get; set; }
    [JsonProperty("image_72")]
    public string Image72 { get; set; }
    [JsonProperty("image_192")]
    public string Image192 { get; set; }
}