namespace SlackNet.Interaction;

public class TextArea : TextElementBase
{
    public TextArea() : base("textarea")
    {
        MaxLength = 3000;
    }
}