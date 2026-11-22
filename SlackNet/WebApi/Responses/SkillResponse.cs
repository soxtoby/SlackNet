#nullable disable
namespace SlackNet.WebApi;

record SkillResponse
{
    public required Skill Skill { get; init; }
}
