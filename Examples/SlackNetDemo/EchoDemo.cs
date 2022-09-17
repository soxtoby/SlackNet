using SlackNet.Interaction;
using SlackNet.WebApi;

namespace SlackNetDemo;

/// <summary>
/// A slash command handler that just echos back whatever you sent it 
/// </summary>
class EchoDemo : ISlashCommandHandler
{
    public const string SlashCommand = "/echo";
    
    public async Task<SlashCommandResponse> Handle(SlashCommand command)
    {
        Console.WriteLine($"{command.UserName} used the {SlashCommand} slash command in the {command.ChannelName} channel");
        
        return new SlashCommandResponse
            {
                Message = new Message
                    {
                        Text = command.Text
                    }
            };
    }
}