namespace SlackNet;

[SlackType("string")]
public class StringField() : EntityField("string")
{
    public string Value { get; set; }
    
    /// <summary>
    /// Can be set to "markdown".
    /// Incompatible with the <see cref="Icon"/> or <see cref="Link"/> properties.
    /// </summary>
    public string Format { get; set; }
    
    /// <summary>
    /// The icon will be rendered to the left of the text.
    /// Not compatible with <see cref="TagColor"/>.
    /// </summary>
    public EntityIcon Icon { get; set; }
    
    /// <summary>
    /// Used to provide a colored "chip" around the text.
    /// </summary>
    public TagColor? TagColor { get; set; }
    
    /// <summary>
    /// The field's content will be hyperlinked with the URL specified here.
    /// </summary>
    public string Link { get; set; }

    /// <summary>
    /// Expands the field across a wider area in the unfurl card.
    /// </summary>
    public bool Long { get; set; }
}

public enum TagColor
{
    Red,
    Yellow,
    Green,
    Gray,
    Blue
}