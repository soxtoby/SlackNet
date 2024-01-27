using System.Collections.Generic;

namespace SlackNet;

public class UserGroupPreferences
{
    public IList<string> Channels { get; set; } = [];
    public IList<string> Groups { get; set; } = [];
}