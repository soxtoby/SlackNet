#nullable enable
namespace SlackNet;

/// <summary>
/// Info on an app.
/// The <see cref="HelpUrl"/>, <see cref="PrivacyPolicyUrl"/>, <see cref="AppHomepageUrl"/>, and <see cref="AppDirectoryUrl"/> properties contain links to more info.
/// </summary>
public class App
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? HelpUrl { get; set; }
    public string? PrivacyPolicyUrl { get; set; }
    public string? AppHomepageUrl { get; set; }
    public string? AppDirectoryUrl { get; set; }
    public bool IsGranularBotApp { get; set; }
    public bool IsAppDirectoryApproved { get; set; }
    public bool IsInternal { get; set; }
    public string? AdditionalInfo { get; set; }
}
