using System.Collections.Generic;

namespace SlackNet;

public class EntityError
{
    public EntityErrorStatus Status { get; set; }
    public string CustomTitle { get; set; }
    public string CustomMessage { get; set; }
    public string MessageFormat { get; set; }
    public IList<EntityActionButton> Actions { get; set; } = [];
}

public enum EntityErrorStatus
{
    NotFound,
    Forbidden,
    GatewayTimeout,
    InternalServerError,
    EditError,
    CustomPartialView
}