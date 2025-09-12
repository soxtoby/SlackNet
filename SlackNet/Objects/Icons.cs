using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet;

public class Icons
{
    /// <summary>
    /// Set to True when no custom icons have been set.
    /// </summary>
    public bool ImageDefault { get; set; }

    [JsonExtensionData]
    private readonly IDictionary<string, JToken> _images = new Dictionary<string, JToken>();

    /// <summary>
    /// A map of icon sizes to image URLs. See the <a href="https://docs.slack.dev/reference/methods/team.info#examples/">team.info documentation</a> for an example.
    /// </summary>
    public IReadOnlyDictionary<string, string> Images => _images.ToDictionary(i => i.Key, i => i.Value.ToObject<string>());
}