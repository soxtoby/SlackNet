using System.Collections.Generic;

namespace SlackNet.WebApi;

public class RoleAssignmentListResponse
{
    public IList<RoleAssignment> RoleAssignments { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
