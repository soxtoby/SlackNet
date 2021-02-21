using Newtonsoft.Json.Linq;

namespace SlackNet.SocketMode
{
    [SlackType("interactive")]
    public class InteractionEnvelope : SocketEnvelope<JObject> { }
}