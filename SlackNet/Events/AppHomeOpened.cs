namespace SlackNet.Events;

/// <summary>
/// Notifies your app when a user has entered into the App Home space - that's the place where a user exchanges DMs with your app.
/// </summary>
public class AppHomeOpened : Event
{
    public string User { get; set; }
    public string Channel { get; set; }
    public AppHomeTab Tab { get; set; }
    public ViewInfo View { get; set; }
}

public enum AppHomeTab
{
    Home,
    Messages
}