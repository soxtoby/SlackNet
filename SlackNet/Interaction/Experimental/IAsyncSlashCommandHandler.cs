using System;
using System.Threading.Tasks;

namespace SlackNet.Interaction.Experimental
{
    [Obsolete(Warning.Experimental)]
    public interface IAsyncSlashCommandHandler
    {
        Task Handle(SlashCommand command, Responder<SlashCommandResponse> respond);
    }
}