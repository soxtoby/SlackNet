using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    abstract class ResolvedSlashCommandHandler : ISlashCommandHandler
    {
        protected ResolvedSlashCommandHandler(string command) => Command = command;

        public string Command { get; }

        public abstract Task<SlashCommandResponse> Handle(SlashCommand command);
    }

    class ResolvedSlashCommandHandler<T> : ResolvedSlashCommandHandler
        where T : ISlashCommandHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public ResolvedSlashCommandHandler(IServiceProvider serviceProvider, string command)
            : base(command)
            => _serviceProvider = serviceProvider;

        public override async Task<SlashCommandResponse> Handle(SlashCommand command)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<T>();
                return await handler.Handle(command).ConfigureAwait(false);
            }
        }
    }
}