namespace SlackNet.WebApi;

public class TokenRotationResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string TeamId { get; set; }
    public string UserId { get; set; }
    public int Exp { get; set; }
    public int Iat { get; set; }
}