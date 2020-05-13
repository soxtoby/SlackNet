using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class SlashCommandHandlerAsyncWrapper : IAsyncSlashCommandHandler
    {
        private readonly ISlashCommandHandler _syncHandler;
        public SlashCommandHandlerAsyncWrapper(ISlashCommandHandler syncHandler) => _syncHandler = syncHandler;

        public async Task Handle(SlashCommand command, Responder<SlashCommandResponse> respond)
        {
            var response = await _syncHandler.Handle(command).ConfigureAwait(false);
            await respond(response).ConfigureAwait(false);
        }
    }
}