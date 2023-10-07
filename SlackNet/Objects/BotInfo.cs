using System;

namespace SlackNet;

public class BotInfo
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string AppId { get; set; }
    public string TeamId { get; set; }
    public string Name { get; set; }
    public Icons Icons { get; set; }
    public bool Deleted { get; set; }
    public int Updated { get; set; }
    public DateTime? UpdatedDateTime => Updated.ToDateTime();
}