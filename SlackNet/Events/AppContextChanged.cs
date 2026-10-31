using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SlackNet.Events;

/// <summary>
/// The context changed while an app was visible.
/// </summary>
public class AppContextChanged : Event
{
    /// <summary>
    /// Information about what the user is currently viewing.
    /// </summary>
    public AppContext Context { get; set; } = new();
}

/// <summary>
/// Describes the Slack entities a user is viewing while interacting with an app.
/// </summary>
public class AppContext : SlackObject
{
    /// <summary>
    /// The entities the user is currently viewing, ordered by relevance.
    /// </summary>
    public IList<AppContextEntity> Entities { get; set; } = [];
}

/// <summary>
/// Identifies one Slack entity in the user's current app context.
/// </summary>
public class AppContextEntity : SlackObject
{
    /// <summary>The kind of entity represented by <see cref="Value"/>.</summary>
    public string Type { get; set; }
    /// <summary>
    /// The entity value. Its JSON shape depends on <see cref="Type"/>; for example, channel IDs are strings while message contexts are objects.
    /// </summary>
    public JToken Value { get; set; }
    /// <summary>The workspace containing the entity.</summary>
    public string TeamId { get; set; }
}
