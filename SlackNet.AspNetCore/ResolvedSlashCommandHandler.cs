using System;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedSlashCommandHandler : ResolvedHandler<ISlashCommandHandler>, ISlashCommandHandler
    {
        public ResolvedSlashCommandHandler(IServiceProvider serviceProvider, Func<IServiceProvider, ISlashCommandHandler> getHandler) 
            : base(serviceProvider, getHandler) { }

        public Task<SlashCommandResponse> Handle(SlashCommand command) => ResolvedHandle(h => h.Handle(command));
    }
}