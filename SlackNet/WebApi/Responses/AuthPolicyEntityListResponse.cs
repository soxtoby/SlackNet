#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AuthPolicyEntityListResponse
{
    /// <summary>
    /// Entities assigned to the policy.
    /// </summary>
    public IList<AuthPolicyEntity> Entities { get; set; } = [];

    /// <summary>
    /// Count of entities under the policy and provided filter conditions.
    /// </summary>
    public int EntityTotalCount { get; set; }

    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
