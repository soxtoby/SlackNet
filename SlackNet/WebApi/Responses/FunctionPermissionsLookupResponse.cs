using System.Collections.Generic;

namespace SlackNet.WebApi;

public class FunctionPermissionsLookupResponse
{
    /// <summary>
    /// The functions' permissions, keyed by function ID.
    /// </summary>
    public IDictionary<string, FunctionPermissions> Permissions { get; set; } = new Dictionary<string, FunctionPermissions>();

    /// <summary>
    /// The functions' metadata, keyed by function ID.
    /// </summary>
    public IDictionary<string, FunctionMetadata> Metadata { get; set; } = new Dictionary<string, FunctionMetadata>();

    /// <summary>
    /// Any errors found during permission lookup, keyed by function ID.
    /// </summary>
    public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
}
