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
        public ActionType Type { get; set; }
        public string Value { get; set; }
        public string Url { get; set; }
        public Confirm Confirm { get; set; }
        public IList<Option> Options { get; set; }
        public IList<OptionGroup> OptionGroups { get; set; }
        public DataSource DataSource { get; set; }
        public IList<Option> SelectedOptions { get; set; } = new List<Option>();

        [JsonIgnore]
        public string SelectedValue
        {
            get => SelectedOptions.FirstOrDefault()?.Value;
            set => SelectedOptions = Options?.Where(o => o.Value == value).ToList()
                ?? new List<Option> { new Option { Value = value } };
        }

        public int MinQueryLength { get; set; } = 1;
    }
}