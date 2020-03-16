using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SlackSlashCommandsService : ISlackSlashCommands
    {
        private readonly SlackSlashCommands _commands = new SlackSlashCommands();

        public SlackSlashCommandsService(IEnumerable<ResolvedSlashCommandHandler> handlers)
        {
            foreach (var handler in handlers) 
                SetHandler(handler.Command, handler);
        }

        public Task<SlashCommandResponse> Handle(SlashCommand command) => _commands.Handle(command);
        public void SetHandler(string command, ISlashCommandHandler handler) => _commands.SetHandler(command, handler);
    }
}