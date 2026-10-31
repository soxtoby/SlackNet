using System.Collections.Generic;

namespace SlackNet;

public class TeamProfile : SlackObject
{
    public IList<TeamProfileField> Fields { get; set; } = [];
}