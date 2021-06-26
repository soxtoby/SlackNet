using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    class SlashCommandHandlerAsyncWrapper : IAsyncSlashCommandHandler
    {
        private readonly ISlashCommandHandler _syncHandler;
        public SlashCommandHandlerAsyncWrapper(ISlashCommandHandler syncHandler) => _syncHandler = syncHandler;

        public async Task Handle(SlashCommand command, Responder<SlashCommandResponse> respond) =>
            await respond(await _syncHandler.Handle(command).ConfigureAwait(false)).ConfigureAwait(false);
    }
}