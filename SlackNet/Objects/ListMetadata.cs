using System.Collections.Generic;
using SlackNet.Blocks;

namespace SlackNet;

public class ListMetadata
{
    public IList<ColumnSchema> Schema { get; set; }
    public IList<ColumnSchema> SubtaskSchema { get; set; }
    public IList<ListView> Views { get; set; }
    public string Icon { get; set; }
    public string Description { get; set; }
    public IList<RichTextBlock> DescriptionBlocks { get; set; }
    public bool IsTrial { get; set; }
    public ListCreationSource CreationSource { get; set; }
    public bool TodoMode { get; set; }
    public string DefaultView { get; set; }
}

public class ListCreationSource
{
    public string Type { get; set; }
    public string ReferenceId { get; set; }
}