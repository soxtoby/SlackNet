using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.WebApi;

namespace SlackNet.EventsExample
{
    public class EchoCommand : ISlashCommandHandler
    {
        public async Task<SlashCommandResponse> Handle(SlashCommand command)
        {
            return new()
                {
                    Message = new Message
                        {
                            Text = command.Text
                        }
                };
        }
    }
}