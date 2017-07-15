using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SlackNet
{
    public class Action
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public Style Style { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public Confirm Confirm { get; set; }
        public IList<Option> Options { get; set; } = new List<Option>();
        public IList<OptionGroup> OptionGroups { get; set; } = new List<OptionGroup>();
        public DataSource DataSource { get; set; }

        [JsonProperty]
        private IList<Option> SelectedOptions { get; set; } = new List<Option>();
        public Option SelectedOption
        {
            get => SelectedOptions.FirstOrDefault();
            set => SelectedOptions = new List<Option> { value };
        }

        public int MinQueryLength { get; set; } = 1;
    }
}