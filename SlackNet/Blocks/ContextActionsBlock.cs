using System.Collections.Generic;

namespace SlackNet.Blocks;

[SlackType("context_actions")]
public class ContextActionsBlock() : Block("context_actions")
{
    public IList<ContextActionsElement> Elements { get; set; } = [];
}

public abstract class ContextActionsElement(string type)
{
    public string Type { get; set; } = type;
}

[SlackType("feedback_buttons")]
public class FeedbackButtons() : ContextActionsElement("feedback_buttons")
{
    public FeedbackButton PositiveButton { get; set; } = new();
    public FeedbackButton NegativeButton { get; set; } = new();
    public string ActionId { get; set; }
}

public class FeedbackButton
{
    public PlainText Text { get; set; }
    public string Value { get; set; }
    public string AccessibilityLabel { get; set; }
}

[SlackType("icon_button")]
public class IconButton() : ContextActionsElement("icon_button")
{
    public string Icon { get; set; }
    public PlainText Text { get; set; }
    public string ActionId { get; set; }
    public string Value { get; set; }
    public ConfirmationDialog Confirm { get; set; }
    public string AccessibilityLabel { get; set; }
    [IgnoreIfEmpty]
    public IList<string> VisibleToUserIds { get; set; } = [];
}