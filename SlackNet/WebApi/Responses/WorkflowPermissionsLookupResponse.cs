#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

record WorkflowPermissionsLookupResponse
{
    public IDictionary<string, WorkflowPermissions> Permissions { get; init; } = new Dictionary<string, WorkflowPermissions>();
}
