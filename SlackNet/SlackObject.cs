using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// The base of the models that SlackNet reads from Slack, so that nothing Slack adds to them is lost before SlackNet has a property for it.
/// </summary>
public abstract class SlackObject
{
    /// <summary>
    /// Anything that Slack includes in the object that isn't covered by other properties can be accessed through this property.
    /// If you find anything here, please raise an issue at https://github.com/soxtoby/SlackNet/issues so we can add it to the library.
    /// These are only read from Slack. They are not written back when the object is sent to Slack.
    /// </summary>
    [JsonExtensionData(WriteData = false)]
    public Dictionary<string, object> ExtraProperties { get; set; } = new();
}
