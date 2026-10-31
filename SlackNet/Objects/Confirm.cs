namespace SlackNet;

public class Confirm : SlackObject
{
    public string Title { get; set; }
    public string Text { get; set; }
    public string OkText { get; set; }
    public string DismissText { get; set; }
}