using System.Collections.Generic;

namespace SlackNet;

public class TeamProfile
{
    public IList<TeamProfileField> Fields { get; set; } = [];
}