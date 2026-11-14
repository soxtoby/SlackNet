namespace SlackNet.WebApi;

record SubteamResponse
{
    public required UserGroup Subteam { get; init; }
}
