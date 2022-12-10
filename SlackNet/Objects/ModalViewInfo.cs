using SlackNet.Blocks;

namespace SlackNet;

[SlackType("modal")]
public class ModalViewInfo : ViewInfo
{
    public PlainText Title { get; set; }
    public PlainText Close { get; set; }
    public PlainText Submit { get; set; }
    public bool ClearOnClose { get; set; }
    public bool NotifyOnClose { get; set; }
    public string PreviousViewId { get; set; }
}