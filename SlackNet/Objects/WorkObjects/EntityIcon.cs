using SlackNet.Blocks;

namespace SlackNet;

public class EntityIcon : SlackObject
{
    public string AltText { get; set; }
    public string Url { get; set; }
    public ImageFileReference SlackFile { get; set; }
}