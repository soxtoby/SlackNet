#nullable disable
using System;
using Newtonsoft.Json;

namespace SlackNet.WebApi;

public class TokenRotationResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string TeamId { get; set; }
    public string UserId { get; set; }
    public int Exp { get; set; }
    [JsonIgnore]
    public DateTime Expires => Exp.ToDateTime().GetValueOrDefault();
    public int Iat { get; set; }
    [JsonIgnore]
    public DateTime IssuedAt => Iat.ToDateTime().GetValueOrDefault();
}