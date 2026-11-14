namespace SlackNet.WebApi;

record BeginShortTokenRotationResponse
{
    public required string NewToken { get; init; }
}
