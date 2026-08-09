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

public class AppContext
{
    /// <summary>
    /// The entities the user is currently viewing, ordered by relevance.
    /// </summary>
    public IList<AppContextEntity> Entities { get; set; } = [];
}

public class AppContextEntity
{
    public string Type { get; set; }
    /// <summary>
    /// The entity value. Its JSON shape depends on <see cref="Type"/>; for example, channel IDs are strings while message contexts are objects.
    /// </summary>
    public JToken Value { get; set; }
    public string TeamId { get; set; }
}
