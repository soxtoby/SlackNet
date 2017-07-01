using System.Collections.Generic;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when users are added or removed from an existing User Group.
    /// Unlike <see cref="SubteamUpdated"/> this only shows the delta of added or removed members and does not include a snapshot of the User Group.
    /// </summary>
    public class SubteamMembersChanged : Event
    {
        public string SubteamId { get; set; }
        public string TeamId { get; set; }
        public int DatePreviousUpdate { get; set; }
        public int DateUpdate { get; set; }
        public IList<string> AddedUsers { get; set; } = new List<string>();
        public string AddedUsersCount { get; set; }
        public IList<string> RemovedUsers { get; set; } = new List<string>();
        public string RemovedUsersCount { get; set; }
    }
}