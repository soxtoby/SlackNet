using System.Collections.Generic;

namespace SlackNet.Objects
{
    public class TeamProfileField
    {
        public string Id { get; set; }
        public int Ordering { get; set; }
        public string Label { get; set; }
        public string Hint { get; set; }
        public string Type { get; set; }
        public IList<string> PossibleValues { get; set; }
        public bool IsHidden { get; set; }
        public ProfileFieldOptions Options { get; set; }
    }
}