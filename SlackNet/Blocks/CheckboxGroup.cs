using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// A checkbox group that allows a user to choose multiple items from a list of possible options.
/// </summary>
[SlackType("checkboxes")]
public class CheckboxGroup : ActionElement, IInputBlockElement
{
    public CheckboxGroup() : base("checkboxes") { }
        
    /// <summary>
    /// An array of <see cref="Option"/> objects.
    /// </summary>
    public IList<Option> Options { get; set; } = [];
        
    /// <summary>
    /// An array of <see cref="Option"/> objects that exactly matches one or more of the options within <see cref="Options"/>.
    /// These options will be selected when the checkbox group initially loads.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<Option> InitialOptions { get; set; } = [];

    /// <summary>
    /// Indicates whether the element will be set to auto focus within the <see cref="ViewInfo"/> object. Only one element can be set to true.
    /// </summary>
    public bool FocusOnLoad { get; set; }
}

[SlackType("checkboxes")]
public class CheckboxGroupAction : BlockAction
{
    public IList<Option> SelectedOptions { get; set; } = [];
}

[SlackType("checkboxes")]
public class CheckboxGroupValue : ElementValue
{
    public IList<Option> SelectedOptions { get; set; } = [];
}