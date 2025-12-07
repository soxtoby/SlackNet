using System.Collections.Generic;

namespace SlackNet.Blocks;

[SlackType("context_actions")]
public class ContextActionsBlock() : Block("context_actions")
{
    public IList<ContextActionsElement> Elements { get; set; } = [];
}

public abstract class ContextActionsElement(string type)
{
    public string Type { get; set; } = type;
    public string ActionId { get; set; }
}