using System.Collections.Generic;

namespace SlackNet.WebApi;

public class TriggerTypePermissionsLookupResponse
{
    /// <summary>
    /// The trigger types' permissions, keyed by trigger type ID.
    /// </summary>
    public IDictionary<string, TriggerTypePermissions> Permissions { get; set; } = new Dictionary<string, TriggerTypePermissions>();

    /// <summary>
    /// The trigger types' metadata, keyed by trigger type ID.
    /// </summary>
    public IDictionary<string, TriggerTypeMetadata> Metadata { get; set; } = new Dictionary<string, TriggerTypeMetadata>();
}
