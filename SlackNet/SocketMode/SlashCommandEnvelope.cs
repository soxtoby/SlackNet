using SlackNet.Interaction;

namespace SlackNet.SocketMode
{
    [SlackType("slash_commands")]
    public class SlashCommandEnvelope : SocketEnvelope<SlashCommand> { }
}