namespace SlackNet.WebApi;

record AdminUserResponse
{
    public required AdminUser User { get; init; }
}
