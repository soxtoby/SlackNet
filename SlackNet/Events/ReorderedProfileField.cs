namespace SlackNet.Events;

public class ReorderedProfileField : SlackObject
{
    public string Id { get; set; }
    public int Ordering { get; set; }
}