namespace SlackNet;

public class Reaction
{
    public string Name { get; set; } = null!;
    public int Count { get; set; }
    public string[]? Users { get; set; }
}