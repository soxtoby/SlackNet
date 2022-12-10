namespace SlackNet.WebApi;

public class OpenIdTokenResponse
{
    public bool Ok { get; set; }
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public string IdToken { get; set; }
    public string State { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
}