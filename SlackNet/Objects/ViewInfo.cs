using System.Collections.Generic;
using SlackNet.Blocks;

namespace SlackNet;

public abstract class ViewInfo
{
    public string Id { get; set; }
    public string TeamId { get; set; }
    public string Type { get; set; }
    public IList<Block> Blocks { get; set; } = [];
    public string PrivateMetadata { get; set; }
    public string CallbackId { get; set; }
    public string ExternalId { get; set; }
    public ViewState State { get; set; }
    public string Hash { get; set; }
    public string RootViewId { get; set; }
    public string AppId { get; set; }
    public string BotId { get; set; }
    public string AppInstalledTeamId { get; set; }
}