using System.Collections.Generic;

namespace SlackNet
{
    public class OptionGroup
    {
        public string Text { get; set; }
        public IList<Option> Options { get; set; }
    }
}