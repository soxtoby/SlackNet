using System;

namespace SlackNet.Blocks
{
    [SlackType("input")]
    public class PlainTextInput : BlockElement, IActionElement
    {
        public PlainTextInput() : base("plain_text_input")
        {
        }

        /// <summary>
        /// An identifier for the input value when the parent modal is submitted
        /// </summary>
        public String ActionId { get; set; }

        /// <summary>
        /// A plain_text only text object that defines the placeholder text shown in the plain-text input. 
        /// </summary>
        public PlainText Placeholder { get; set; }

        /// <summary>
        /// The initial value in the plain-text input when it is loaded.
        /// </summary>
        public String InitialValue { get; set; }

        /// <summary>
        /// Indicates whether the input will be a single line (false) or a larger textarea (true). 
        /// </summary>
        public Boolean? Multiline { get; set; }

        /// <summary>
        /// The minimum length of input that the user must provide. If the user provides less, they will receive an error
        /// </summary>
        public Int32? MinLength { get; set; }

        /// <summary>
        /// The maximum length of input that the user can provide. If the user provides more, they will receive an error.
        /// </summary>
        public Int32? MaxLength { get; set; }
    }
}