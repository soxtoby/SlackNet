namespace SlackNet.Events;

public class AssistantThreadStarted : Event
{
    public AssistantThread AssistantThread { get; set; } = new();
}