using SlackNet.Blocks;

namespace SlackNet;

[SlackType("slack#/types/image")]
public class ImageField() : EntityField("slack#/types/image")
{
    public string AltText { get; set; }
    public string ImageUrl { get; set; }
    public ImageFileReference SlackFile { get; set; }
}