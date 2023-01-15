using System.Collections.Generic;

namespace SlackNet.Blocks;

public class MessageBlocks {
    public IList<Block> Blocks { get; set; }
}

public class MessageBlock
{
    public string Team { get; set; }
    public string Channel { get; set; }
    public string Ts { get; set; }
    public MessageBlocks Message { get; set; }
}
