using System.Collections.Generic;

namespace SlackNet.Blocks
{
    /// <summary>
    /// A radio button group that allows a user to choose one item from a list of possible options.
    /// </summary>
    [SlackType("radio_buttons")]
    public class RadioButtonGroup : ActionElement, IInputBlockElement
    {
        public RadioButtonGroup() : base("radio_buttons") { }
        
        /// <summary>
        /// An array of <see cref="Option"/> objects.
        /// </summary>
        public IList<Option> Options { get; set; } = new List<Option>();

        /// <summary>
        /// An <see cref="Option"/> object that exactly matches one of the options within <see cref="Options"/>.
        /// This option will be selected when the radio button group initially loads.
        /// </summary>
        public Option InitialOption { get; set; }

        /// <summary>
        /// Indicates whether the element will be set to auto focus within the <see cref="ViewInfo"/> object. Only one element can be set to true.
        /// </summary>
        public bool FocusOnLoad { get; set; }
    }

    [SlackType("radio_buttons")]
    public class RadioButtonGroupAction : BlockAction
    {
        public Option SelectedOption { get; set; }
    }

    [SlackType("radio_buttons")]
    public class RadioButtonGroupValue : ElementValue
    {
        public Option SelectedOption { get; set; }
    }
}