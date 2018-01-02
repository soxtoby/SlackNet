using System.Collections.Generic;

namespace SlackNet.Interaction
{
    public class SelectElement : DialogElement
    {
        public SelectElement() : base(DialogElementType.Select) { }

        public IList<SelectOption> Options { get; set; } = new List<SelectOption>();
    }
}