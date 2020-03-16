using System.Collections.Generic;

namespace SlackNet.Blocks
{
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
        public IList<Option> Options { get; set; } = new List<Option>();
        
        /// <summary>
        /// An array of <see cref="Option"/> objects that exactly matches one or more of the options within <see cref="Options"/>.
        /// These options will be selected when the checkbox group initially loads.
        /// </summary>
        [IgnoreIfEmpty]
        public IList<Option> InitialOptions { get; set; } = new List<Option>();
    }
}