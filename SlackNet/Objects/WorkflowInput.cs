using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet
{
    public class WorkflowInput
    {
        public object Value { get; set; }

        public bool? SkipVariableReplacement { get; set; }

        public Args Variables { get; set; }
    }
}