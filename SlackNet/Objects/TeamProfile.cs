using System.Collections.Generic;

namespace SlackNet.Objects
{
    public class TeamProfile
    {
        public IList<TeamProfileField> Fields { get; set; } = new List<TeamProfileField>();
    }
}