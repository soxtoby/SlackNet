namespace SlackNet.WebApi;

public class OauthAccessResponse : SlackObject
{
    public string AccessToken { get; set; }
    public string Scope { get; set; }
}