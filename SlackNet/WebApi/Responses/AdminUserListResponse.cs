#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AdminUserListResponse
{
    public IList<AdminUser> Users { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
