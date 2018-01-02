using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet.WebApi
{
    public class WebApiResponse
    {
        public bool Ok { get; set; }
        [JsonExtensionData]
        public JObject Data { get; set; }
    }
}