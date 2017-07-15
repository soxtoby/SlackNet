using System;

namespace SlackNet.WebApi
{
    public class DndResponse
    {
        public bool DndEnabled { get; set; }
        public int NextDndStartTs { get; set; }
        public DateTime? NextDndStart => NextDndStartTs.ToDateTime();
        public int NextDndEndTs { get; set; }
        public DateTime? NextDndEnd => NextDndEndTs.ToDateTime();
    }
}