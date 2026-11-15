using System;
using Newtonsoft.Json.Linq;
using SlackNet.Interaction;

namespace SlackNet.SocketMode;

/// <remarks>
/// Slack mixes <see cref="InteractionRequest"/>, <see cref="OptionsRequest"/>, and <see cref="BlockOptionsRequest"/> together in the same envelope type,
/// and because <see cref="InteractiveMessage"/> and <see cref="OptionsRequest"/> have the same type name, there's no way to automatically deserialize the correct payload type.
/// </remarks>
[SlackType("interactive")]
[Obsolete("Socket mode messages are handled by the socket mode client from SlackServiceBuilder or AddSlackNet. This will become internal in a future version.")]
public class InteractionEnvelope : SocketEnvelope<JObject>;