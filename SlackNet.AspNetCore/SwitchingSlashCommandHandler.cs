using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SwitchingSlashCommandHandler : ISlashCommandHandler
    {
        private readonly Dictionary<string, ISlashCommandHandler> _handlers;
        public SwitchingSlashCommandHandler(IEnumerable<KeyedItem<ISlashCommandHandler>> handlers) =>
            _handlers = handlers.ToDictionary(h => h.Key, h => h.Item);

        public Task<SlashCommandResponse> Handle(SlashCommand command) =>
            _handlers.TryGetValue(command.Command, out var handler)
                ? handler.Handle(command)
                : Task.FromResult((SlashCommandResponse) null);
    }
}