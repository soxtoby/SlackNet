using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface ISlashCommandHandler
    {
        Task<SlashCommandResponse> Handle(SlashCommand command);
    }
}