using System.Collections.Generic;

namespace SlackNet.Blocks;

[SlackType("icon_button")]
public class IconButton() : ContextActionsElement("icon_button")
{
    public string Icon { get; set; }
    public PlainText Text { get; set; }
    public string Value { get; set; }
    public ConfirmationDialog Confirm { get; set; }
    public string AccessibilityLabel { get; set; }
    [IgnoreIfEmpty]
    public IList<string> VisibleToUserIds { get; set; } = [];
}

[SlackType("icon_button")]
public class IconButtonAction : BlockAction
{
    public string Icon { get; set; }
    public PlainText Text { get; set; }
    public string Value { get; set; }
}