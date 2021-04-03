using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public class SwitchingSlashCommandHandler : IAsyncSlashCommandHandler, IComposedHandler<SlashCommand>
    {
        private readonly IHandlerIndex<IAsyncSlashCommandHandler> _handlers;
        public SwitchingSlashCommandHandler(IHandlerIndex<IAsyncSlashCommandHandler> handlers) => _handlers = handlers;

        public Task Handle(SlashCommand command, Responder<SlashCommandResponse> respond) =>
            _handlers.TryGetHandler(command.Command, out var handler)
                ? handler.Handle(command, respond)
                : Task.FromResult((SlashCommandResponse) null);

        IEnumerable<object> IComposedHandler<SlashCommand>.InnerHandlers(SlashCommand request) =>
            _handlers.TryGetHandler(request.Command, out var handler)
                ? handler.InnerHandlers(request)
                : Enumerable.Empty<object>();
    }
}