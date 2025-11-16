using System.Collections.Generic;
using SlackNet.Blocks;

namespace SlackNet;

public class EntityEditSupport
{
    public bool Enabled { get; set; }
    public PlainText Placeholder { get; set; }
    public PlainText Hint { get; set; }
    public bool Optional { get; set; }
    public SelectEditing Select { get; set; }
    public NumberEditing Number { get; set; }
    public TextEditing Text { get; set; }
}

public class SelectEditing
{
    /// <summary>
    /// The current value of the select input.
    /// </summary>
    public string CurrentValue { get; set; }

    [IgnoreIfEmpty]
    public IList<string> CurrentValues { get; set; } = [];

    [IgnoreIfEmpty]
    public IList<EntitySelectOption> StaticOptions { get; set; } = [];

    public bool FetchOptionsDynamically { get; set; }
}

public class EntitySelectOption
{
    /// <summary>
    /// The value that will be sent to your application when the user submits the form.
    /// </summary>
    public string Value { get; set; }
    
    /// <summary>
    /// Human-readable text that will be displayed in the select menu.
    /// </summary>
    public TextObject Text { get; set; }
    
    /// <summary>
    /// A descriptive text that will be shown below the text property.
    /// </summary>
    public TextObject Description { get; set; }
}

public class NumberEditing
{
    /// <summary>
    /// Minimum allowed value.
    /// </summary>
    public double MinValue { get; set; }
    
    /// <summary>
    /// Maximum allowed value.
    /// </summary>
    public double MaxValue { get; set; }
}

public class TextEditing
{
    /// <summary>
    /// Minimum string length.
    /// </summary>
    public int MinLength { get; set; }
    
    /// <summary>
    /// Maximum string length.
    /// </summary>
    public int MaxLength { get; set; }
}