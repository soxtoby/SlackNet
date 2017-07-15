using System;
using System.Collections.Generic;

namespace SlackNet.WebApi
{
    public class UserListResponse
    {
        public IList<User> Members { get; set; }
        public int CacheTs { get; set; }
        public DateTime CacheTime => CacheTs.ToDateTime().GetValueOrDefault();
        public UserListResponseMetadata ResponseMetadata { get; set; }
    }
}