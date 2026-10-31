#nullable enable
using System.Collections.Generic;
using SlackNet.WebApi;

namespace SlackNet;

public class LookupCriteria : SlackObject
{
    public IList<SectionType> SectionTypes { get; set; } = [];
    public string? ContainsText { get; set; }
}

public enum SectionType
{
    H1,
    H2,
    H3,
    AnyHeader
}