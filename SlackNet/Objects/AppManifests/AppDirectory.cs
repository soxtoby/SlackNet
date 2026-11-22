using System.Collections.Generic;

namespace SlackNet.AppManifests;

/// <summary>
/// Information to be listed in the Slack Marketplace.
/// </summary>
public class AppDirectory
{
    /// <summary>
    /// Categories to list the app in.
    /// </summary>
    public IList<string>? AppDirectoryCategories { get; set; }

    /// <summary>
    /// Whether the app should use direct install.
    /// </summary>
    public bool? UseDirectInstall { get; set; }

    /// <summary>
    /// The URL of the install page.
    /// </summary>
    public string? DirectInstallUrl { get; set; }

    /// <summary>
    /// The URL of the installation landing page.
    /// </summary>
    public required string InstallationLandingPage { get; set; }

    /// <summary>
    /// A link to your app's privacy policy.
    /// </summary>
    public required string PrivacyPolicyUrl { get; set; }

    /// <summary>
    /// A link to your app's support URL.
    /// </summary>
    public required string SupportUrl { get; set; }

    /// <summary>
    /// An email address to contact your app's support.
    /// </summary>
    public required string SupportEmail { get; set; }

    /// <summary>
    /// The languages supported by the app.
    /// </summary>
    public IList<string> SupportedLanguages { get; set; } = [];

    /// <summary>
    /// Pricing information.
    /// </summary>
    public required string Pricing { get; set; }
}
