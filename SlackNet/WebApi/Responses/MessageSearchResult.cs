using System.Collections.Generic;
using Newtonsoft.Json;
using SlackNet.Blocks;

namespace SlackNet.WebApi;

public class MessageSearchResult
{
    public string Iid { get; set; }
    public string Team { get; set; }
    public double Score { get; set; }
    public Conversation Channel { get; set; }
    public string Type { get; set; }
    public string User { get; set; }
    public string Username { get; set; }
    public string Ts { get; set; }
    public IList<Attachment> Attachments { get; set; } = [];
    public IList<Block> Blocks { get; set; } = [];
    public string Text { get; set; }
    public string Permalink { get; set; }
    public bool NoReactions { get; set; }

    /// <summary>
    /// Anything that Slack includes in the search result that isn't covered by other properties can be accessed through this property.
    /// If you find anything here, please raise an issue at https://github.com/soxtoby/SlackNet/issues so we can add it to the library.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object> ExtraProperties { get; set; } = new();
}