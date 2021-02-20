using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public class SwitchingSlashCommandHandler : IAsyncSlashCommandHandler
    {
        private readonly IHandlerIndex<IAsyncSlashCommandHandler> _handlers;
        public SwitchingSlashCommandHandler(IHandlerIndex<IAsyncSlashCommandHandler> handlers) => _handlers = handlers;

        public Task Handle(SlashCommand command, Responder<SlashCommandResponse> respond) =>
            _handlers.TryGetValue(command.Command, out var handler)
                ? handler.Handle(command, respond)
                : Task.FromResult((SlashCommandResponse) null);
    }
}