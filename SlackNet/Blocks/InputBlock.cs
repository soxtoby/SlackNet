using System;

namespace SlackNet.Blocks
{
    /// <summary>
    /// A simple image block, designed to make those cat photos really pop.
    /// </summary>
    [SlackType("input")]
    public class InputBlock : Block
    {
        public InputBlock() : base("input")
        {
        }

        /// <summary>
        /// A label that appears above an input element in the form of a text object that must have type of plain_text.
        /// </summary>
        public PlainText Label { get; set; }

        /// <summary>
        /// An plain-text input element, a select menu element, a multi-select menu element, or a datepicker.
        /// </summary>
        public BlockElement Element { get; set; }

        /// <summary>
        /// An optional hint that appears below an input element in a lighter grey
        /// </summary>
        public PlainText Hint { get; set; }

        /// <summary>
        /// A boolean that indicates whether the input element may be empty when a user submits the modal. 
        /// </summary>
        public Boolean? Optional { get; set; }
    }
}