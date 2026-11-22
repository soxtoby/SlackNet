using System.Collections.Generic;

namespace SlackNet.WebApi;

class LookupResponse
{
    public IList<CanvasSection> Sections { get; set; } = [];
}