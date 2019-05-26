namespace SlackNet.Interaction
{
    public class TextArea : TextElementBase
    {
        public TextArea() : base(DialogElementType.TextArea)
        {
            MaxLength = 3000;
        }
    }
}