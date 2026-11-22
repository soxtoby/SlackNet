namespace SlackNet.AppManifests;

/// <summary>
/// A group of settings that describe an <see cref="AppManifest"/>.
/// </summary>
public class Metadata
{
    /// <summary>
    /// The major version of the manifest schema to target.
    /// </summary>
    public int? MajorVersion { get; set; }

    /// <summary>
    /// The minor version of the manifest schema to target.
    /// </summary>
    public int? MinorVersion { get; set; }
}
