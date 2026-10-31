namespace SlackNet;

public class Reaction : SlackObject
{
    public string Name { get; set; }
    public int Count { get; set; }
    public string[] Users { get; set; }
}