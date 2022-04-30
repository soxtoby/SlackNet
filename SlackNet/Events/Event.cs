using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.Events
{
    public class Event
    {
        public string Type { get; set; }

        /// <summary>
        /// Anything that Slack includes in the event that isn't covered by other properties can be accessed through this property.
        /// If you find anything here, please raise an issue at https://github.com/soxtoby/SlackNet/issues so we can add it to the library.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtraProperties { get; set; } = new();
    }
}