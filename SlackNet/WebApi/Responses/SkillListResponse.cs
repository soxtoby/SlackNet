#nullable disable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class SkillListResponse
{
    public IList<Skill> Skills { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
