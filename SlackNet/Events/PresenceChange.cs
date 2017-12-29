using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when a user changes presence status.
    /// </summary>
    public class PresenceChange : Event
    {
        public IList<string> Users { get; set; }
        [JsonIgnore]
        public string User
        {
            get => Users?.First();
            set => Users = new List<string> { value };
        }
        public Presence Presence { get; set; }
    }
}