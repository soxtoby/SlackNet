using System.Runtime.Serialization;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when a the current billing plan is changed.
    /// </summary>
    public class TeamPlanChange : Event
    {
        public Plan Plan { get; set; }
    }

    public enum Plan
    {
        [EnumMember(Value = "std")] Standard,
        Plus
    }
}