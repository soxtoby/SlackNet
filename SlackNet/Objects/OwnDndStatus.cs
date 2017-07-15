namespace SlackNet
{
    public class OwnDndStatus : DndStatus
    {
        public bool SnoozeEnabled { get; set; }
        public int SnoozeEndtime { get; set; }
    }
}