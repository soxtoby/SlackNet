using System;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class ResolvedSlashCommandHandler : ResolvedHandler<IAsyncSlashCommandHandler>, IAsyncSlashCommandHandler
    {
        public ResolvedSlashCommandHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IAsyncSlashCommandHandler> getHandler) 
            : base(serviceProvider, getHandler) { }

        public Task Handle(SlashCommand command, Responder<SlashCommandResponse> respond) => ResolvedHandle(h => h.Handle(command, respond));
    }
}