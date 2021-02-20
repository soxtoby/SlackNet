namespace SlackNet.Blocks
{
    [SlackType("plain_text")]
    public class PlainText : TextObject, IContextElement
    {
        public PlainText(string text = "") : base("plain_text")
        {
            Text = text;
        }

        /// <summary>
        /// Indicates whether emojis in a text field should be escaped into the colon emoji format.
        /// </summary>
        public bool Emoji { get; set; }

        public override string ToString() => Text;

        public static implicit operator PlainText(string text) => new(text);
    }
}