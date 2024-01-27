using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.Blocks;

public class FileInput : BlockElement, IInputBlockElement
{
    public FileInput() : base("file_input") { }

    /// <summary>
    /// An identifier for this action. You can use this when you receive an interaction payload to identify the source of the action.
    /// Should be unique among all other <see cref="ActionId"/>s used elsewhere by your app. 
    /// </summary>
    public string ActionId { get; set; }

    /// <summary>
    /// An array of valid file extensions that will be accepted for this element.
    /// All file extensions will be accepted if filetypes is not specified.
    /// This validation is provided for convenience only, and you should
    /// perform your own file type validation based on what you expect to receive.
    /// </summary>
    [IgnoreIfEmpty]
    [JsonProperty("filetypes")]
    public IList<string> FileTypes { get; set; } = new List<string>();

    /// <summary>
    /// Maximum number of files that can be uploaded for this element.
    /// Minimum of 1, maximum of 10.
    /// Defaults to 10 if not specified.
    /// </summary>
    public int MaxFiles { get; set; }
}

[SlackType("file_input")]
public class FileInputValue : ElementValue
{
    public IList<File> Files { get; set; }
}