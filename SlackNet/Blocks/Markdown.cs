namespace SlackNet.Blocks
{
    [SlackType("mrkdwn")]
    public class Markdown : TextObject, IContextElement
    {
        public Markdown(string markdown = "") : base("mrkdwn")
        {
            Text = markdown;
        }

        /// <summary>
        /// When set to false (as is default) URLs will be auto-converted into links, conversation names will be link-ified, and certain mentions will be automatically parsed. 
        /// Using a value of true will skip any preprocessing of this nature, although you can still include manual parsing strings.
        /// </summary>
        public bool Verbatim { get; set; }

        public override string ToString() => Text;

        public static implicit operator Markdown(string text) => new(text);
    }
}