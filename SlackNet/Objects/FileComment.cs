using System.Collections.Generic;

namespace SlackNet;

public class FileComment
{
    public string Id { get; set; }
    public int Created { get; set; }
    public int Timestamp { get; set; }
    public string User { get; set; }
    public string Comment { get; set; }
    public string Channel { get; set; }
    public IList<Reaction> Reactions { get; set; }
}