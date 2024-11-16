namespace SlackNet.Events;

public class AssistantThreadContextChanged : Event
{
    public AssistantThread AssistantThread { get; set; } = new();
}