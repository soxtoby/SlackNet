namespace SlackNet.Interaction
{
    public abstract class DialogElement
    {
        protected DialogElement(DialogElementType type) => Type = type;

        public DialogElementType Type { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public bool Optional { get; set; }
        public string Value { get; set; }
        public string Placeholder { get; set; }
    }
}