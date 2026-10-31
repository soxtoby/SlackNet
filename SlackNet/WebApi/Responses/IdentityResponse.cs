namespace SlackNet.WebApi;

public class IdentityResponse : SlackObject
{
    public UserIdentity User { get; set; }
    public TeamIdentity Team { get; set; }
}