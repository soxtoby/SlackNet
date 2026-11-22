#nullable disable
namespace SlackNet.WebApi;

record AgentSessionRenameResponse
{
    public required string Title { get; init; }
}
