#nullable enable
namespace SlackNet.AppManifests;

/// <summary>
/// A group of settings that describe parts of an app's appearance within Slack.
/// </summary>
public class DisplayInformation
{
    /// <summary>
    /// The name of the app.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// A short description of the app for display to users.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// A longer version of the description of the app.
    /// </summary>
    public string? LongDescription { get; set; }

    /// <summary>
    /// A hex color value (including the hex sign) that specifies the background color used when Slack displays information about your app.
    /// Can be 3-digit (<c>#000</c>) or 6-digit (<c>#000000</c>) hex values.
    /// Once a background color value is set, it cannot be removed, only updated.
    /// </summary>
    public string? BackgroundColor { get; set; }
}
