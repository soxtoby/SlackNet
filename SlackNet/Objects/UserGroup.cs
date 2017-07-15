using System;
using System.Collections.Generic;

namespace SlackNet
{
    public class UserGroup
    {
        public string Id { get; set; }
        public string TeamId { get; set; }
        public bool IsUsergroup { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Handle { get; set; }
        public bool IsExternal { get; set; }
        public int DateCreate { get; set; }
        public DateTime Created => DateCreate.ToDateTime().GetValueOrDefault();
        public int DateUpdate { get; set; }
        public DateTime? Updated => DateUpdate.ToDateTime();
        public int DateDelete { get; set; }
        public DateTime? Deleted => DateDelete.ToDateTime();
        public UserGroupAutoType? AutoType { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public UserGroupPreferences Prefs { get; set; }
        public IList<string> Users { get; set; } = new List<string>();
        public int UserCount { get; set; }
    }
}