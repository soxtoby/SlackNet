using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class SwitchingSlashCommandHandler : IAsyncSlashCommandHandler
    {
        private readonly Dictionary<string, IAsyncSlashCommandHandler> _handlers;
        public SwitchingSlashCommandHandler(IEnumerable<KeyedItem<IAsyncSlashCommandHandler>> handlers) =>
            _handlers = handlers.ToDictionary(h => h.Key, h => h.Item);

        public Task Handle(SlashCommand command, Responder<SlashCommandResponse> respond) =>
            _handlers.TryGetValue(command.Command, out var handler)
                ? handler.Handle(command, respond)
                : Task.FromResult((SlashCommandResponse) null);
    }
}