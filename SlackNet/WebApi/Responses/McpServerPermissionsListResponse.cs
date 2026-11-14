#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

record McpServerPermissionsListResponse
{
    public IList<McpServerPermissions> Servers { get; init; } = [];
}
