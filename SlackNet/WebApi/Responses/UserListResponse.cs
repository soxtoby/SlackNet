using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.WebApi;

public class UserListResponse
{
    public IList<User> Members { get; set; } = [];
    public int CacheTs { get; set; }
    [JsonIgnore]
    public DateTime CacheTime => CacheTs.ToDateTime().GetValueOrDefault();
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}