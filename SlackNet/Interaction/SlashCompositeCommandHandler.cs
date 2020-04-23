using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public class SlashCompositeCommandHandler : ISlashCommandHandler
    {
        readonly ISlackSlashCommands _slackSlashCommands;

        public SlashCompositeCommandHandler(ISlackSlashCommands slackSlashCommands) => _slackSlashCommands = slackSlashCommands;

        public async Task<SlashCommandResponse> Handle(SlashCommand command) => await _slackSlashCommands.Handle(NextCommand(command)).ConfigureAwait(false);

        protected virtual SlashCommand NextCommand(SlashCommand command)
        {
            var text = command.Text;
            var idx = text.IndexOf(' ');
            if (idx == -1)
            {
                command.Command += $" {text}";
                command.Text = null;
                return command;
            }
            command.Command += $" {text.Substring(0, idx)}";
            command.Text = text.Substring(idx + 1);
            return command;
        }
    }
}
