#nullable disable
using System;
using Newtonsoft.Json;

namespace SlackNet.WebApi;

public class Login
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public int DateFirst { get; set; }
    [JsonIgnore]
    public DateTime First => DateFirst.ToDateTime().GetValueOrDefault();
    public int DateLast { get; set; }
    [JsonIgnore]
    public DateTime Last => DateLast.ToDateTime().GetValueOrDefault();
    public int Count { get; set; }
    public string Ip { get; set; }
    public string UserAgent { get; set; }
    public string Isp { get; set; }
    public string Country { get; set; }
    public string Region { get; set; }
}