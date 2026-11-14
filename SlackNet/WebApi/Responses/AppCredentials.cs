#nullable enable
namespace SlackNet.WebApi;

/// <summary>
/// The credentials of an app created with <see cref="IAppsManifestApi.Create"/>.
/// </summary>
public class AppCredentials
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? VerificationToken { get; set; }
    public string? SigningSecret { get; set; }
}
