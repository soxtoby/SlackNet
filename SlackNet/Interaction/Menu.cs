using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SlackNet.Interaction;

[SlackType("select")]
public class Menu : ActionElement
{
    public Menu() : base("select") { }

    public IList<Option> Options { get; set; }
    public IList<OptionGroup> OptionGroups { get; set; }
    public DataSource DataSource { get; set; }
    public int MinQueryLength { get; set; } = 1;
    public IList<Option> SelectedOptions { get; set; } = new List<Option>();

    [JsonIgnore]
    public string SelectedValue
    {
        get => SelectedOptions.FirstOrDefault()?.Value;
        set => SelectedOptions = Options?.Where(o => o.Value == value).ToList()
            ?? new List<Option> { new() { Value = value } };
    }
}