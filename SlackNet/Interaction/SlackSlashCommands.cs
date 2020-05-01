using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface ISlackSlashCommands
    {
        Task<SlashCommandResponse> Handle(SlashCommand command);
        void SetHandler(string command, ISlashCommandHandler handler);
    }

    public class SlackSlashCommands : ISlackSlashCommands
    {
        private readonly Dictionary<string, ISlashCommandHandler> _handlers = new Dictionary<string, ISlashCommandHandler>();

        public Task<SlashCommandResponse> Handle(SlashCommand command) =>
            _handlers.TryGetValue(command.Command, out var handler)
                ? handler.Handle(command)
                : Task.FromResult((SlashCommandResponse)null);

        public void SetHandler(string command, ISlashCommandHandler handler)
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'.", nameof(command));
            
            _handlers[command] = handler;
        }
    }
}