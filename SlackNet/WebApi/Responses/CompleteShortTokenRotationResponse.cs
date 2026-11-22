#nullable disable
namespace SlackNet.WebApi;

record CompleteShortTokenRotationResponse
{
    public required string Token { get; init; }
}
