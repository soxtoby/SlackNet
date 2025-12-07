namespace SlackNet.Blocks;

[SlackType("feedback_buttons")]
public class FeedbackButtons() : ContextActionsElement("feedback_buttons")
{
    public FeedbackButton PositiveButton { get; set; } = new();
    public FeedbackButton NegativeButton { get; set; } = new();
}

public class FeedbackButton
{
    public PlainText Text { get; set; }
    public string Value { get; set; }
    public string AccessibilityLabel { get; set; }
}

[SlackType("feedback_buttons")]
public class FeedbackButtonsAction : BlockAction
{
    public PlainText Text { get; set; }
    public string Value { get; set; }
}