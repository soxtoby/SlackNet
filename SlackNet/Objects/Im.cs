using System;

namespace SlackNet.Objects
{
    public class Im
    {
        public string Id { get; set; }
        public bool IsIm { get; set; }
        public string User { get; set; }
        public int Created { get; set; }
        public DateTime CreatedDateTime => Created.ToDateTime().GetValueOrDefault();
        public bool IsUserDeleted { get; set; }
    }
}