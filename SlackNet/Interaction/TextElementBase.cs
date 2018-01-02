namespace SlackNet.Interaction
{
    public abstract class TextElementBase : DialogElement
    {
        protected TextElementBase(DialogElementType type) : base(type) { }
        public InputElementType? Subtype { get; set; }
        public int MaxLength { get; set; } = 150;
        public int MinLength { get; set; }
        public string Hint { get; set; }
    }
}