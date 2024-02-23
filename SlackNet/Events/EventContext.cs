namespace SlackNet.Events;

public class EventContext (EventCallback eventCallback)
{
    public EventCallback EventCallback { get; } = eventCallback;
}