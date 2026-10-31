namespace SlackNet;

public class Topic : SlackObject
{
    public string Value { get; set; }
    public string Creator { get; set; }
    public int LastSet { get; set; }
}