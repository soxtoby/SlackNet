using System;
using Newtonsoft.Json;

namespace SlackNet.WebApi;

public class TokenRotationResponse
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string TeamId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public int Exp { get; set; }
    [JsonIgnore]
    public DateTime Expires => Exp.ToDateTime().GetValueOrDefault();
    public int Iat { get; set; }
    [JsonIgnore]
    public DateTime IssuedAt => Iat.ToDateTime().GetValueOrDefault();
}