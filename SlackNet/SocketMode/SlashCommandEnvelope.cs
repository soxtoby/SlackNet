using System;
using SlackNet.Interaction;

namespace SlackNet.SocketMode;

[SlackType("slash_commands")]
[Obsolete("Socket mode messages are handled by the socket mode client from SlackServiceBuilder or AddSlackNet. This will become internal in a future version.")]
public class SlashCommandEnvelope : SocketEnvelope<SlashCommand>;