using System.Collections.Generic;

namespace SlackNet.Interaction;

public class SelectElement : DialogElement
{
    public SelectElement() : base("select") { }

    public IList<SelectOption> Options { get; set; } = new List<SelectOption>();
    public DataSource DataSource { get; set; }
}