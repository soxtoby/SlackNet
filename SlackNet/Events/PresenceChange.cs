using System.Collections.Generic;
using System.Linq;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when a user changes presence status.
    /// </summary>
    public class PresenceChange : Event
    {
        public IList<string> Users { get; set; }
        public string User
        {
            get => Users?.First();
            set => Users = new List<string> { value };
        }
        public Presence Presence { get; set; }
    }
}