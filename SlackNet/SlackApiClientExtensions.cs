using System.Threading;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet;

public static class SlackApiClientExtensions
{
    public static Task Respond(this ISlackApiClient client, InteractionRequest request, MessageResponse response, CancellationToken? cancellationToken = null) => 
        client.Respond(request.ResponseUrl, new MessageUpdateResponse(response), cancellationToken);

    public static Task Respond(this ISlackApiClient client, SlashCommand slashCommand, SlashCommandResponse response, CancellationToken? cancellationToken = null) => 
        client.Respond(slashCommand.ResponseUrl, new SlashCommandMessageResponse(response), cancellationToken);
}